# Callers to neural style algorithms
# Callers to neural style algorithms
from subprocess import call
from itertools import product
from tempfile import TemporaryDirectory, NamedTemporaryFile
from shutil import copyfile
import logging
from math import ceil
import numpy as np
import json
import GPUtil
import datetime
from neuralstyle.utils import filename, fileext
from neuralstyle.imagemagick import (convert, resize, shape, assertshape, choptiles, feather, smush, composite, extractalpha, mergealpha)

logging.basicConfig(level=logging.ERROR)
LOGGER = logging.getLogger(__name__)

# Folders and commands for style-transfer algorithms
ALGORITHMS = {
    "gatys": {
        "folder": "/app/neural-style",
        "command": "th neural_style.lua",
        "defaultpars": [
            "-backend", "cudnn",
            "-cudnn_autotune",
            "-normalize_gradients",
            "-init", "image",
            "-content_weight", "100",
            "-save_iter", "10000",
	    "-print_iter", "50",
            "-proto_file", "/app/neural-style/models/VGG_ILSVRC_19_layers_deploy.prototxt",
            "-model_file", "/app/neural-style/models/VGG_ILSVRC_19_layers.caffemodel",
            "-num_iterations", "500"
        ]
    },
    "gatys-multiresolution": {},
    "chen-schmidt": {
        "folder": "/app/style-swap",
        "command": "th style-swap.lua",
        "defaultpars": [
            "--patchSize", "7",
            "--patchStride", "3"
        ]
    },
    "chen-schmidt-inverse": {
        "folder": "/app/style-swap",
        "command": "th style-swap.lua",
        "defaultpars": [
            "--decoder", "models/dec-tconv-sigmoid.t7"
        ]
    }
}


def styletransfer(contents, styles, outfile, size, alg, iterations, weights, stylescales, tilesize, tileoverlap, colors, otherparams):
    """General style transfer routine over multiple sets of options"""
    # Check arguments
    if alg not in ALGORITHMS.keys():
        raise ValueError("Unrecognized algorithm %s, must be one of %s" % (alg, str(list(ALGORITHMS.keys()))))

    for color in colors:
        if color not in [0, 1]:
            raise ValueError("Unnaceptable colors. Only use 0 or 1")

    if weights is None:
    	weights = [5.0]
    if stylescales is None:
    	stylescales = [1.0]
    if tileoverlap is None:
        tileoverlap = 100
    if tilesize is None:
        tilesize = 700
    if otherparams is None:
        otherparams = []
    if colors is None:
        colors = [0]
    if iterations is None:
        iterations = [500]
    
    # Iterate through all combinations
    for content, style, weight, stylescale, iteration, color in product(contents, styles, weights, stylescales, iterations, colors):
        LOGGER.info("working on %s, style=%s, sw=%d, ss=%d, iterations=%d, tilesize=%d, tileoverlap=%d, color=%d" % (content, style, weight, stylescale, iteration, tilesize, tileoverlap, color))

        if outfile is None:
            outfile = outname(savefolder, content, style, alg, iteration, size, stylescale, weight, color)

        LOGGER.info("%s" % outfile)

        # If the desired size is smaller than the maximum tile size, use a direct neural style
        if fitsingletile(targetshape(content, size), alg, tilesize):
            styletransfer_single(content, style, outfile, size, alg, iteration, weight, stylescale, color, otherparams)
        # Else use a tiling strategy
        else:
            neuraltile(content, style, outfile, size, tilesize, tileoverlap, alg, iteration, weight, stylescale, color, otherparams)


def styletransfer_single(content, style, outfile, size, alg, iteration, weight, stylescale, color, otherparams):
    """General style transfer routine over a single set of options"""
    workdir = TemporaryDirectory()

    # Cut out alpha channel from content
    rgbfile = workdir.name + "/" + "rgb.png"
    alphafile = workdir.name + "/" + "alpha.png"
    extractalpha(content, rgbfile, alphafile)

    # Transform style to png, as some algorithms don't understand other formats
    stylepng = workdir.name + "/" + "style.png"
    convert(style, stylepng)

    # Call style transfer algorithm
    algfile = workdir.name + "/" + "algoutput.png"
    if alg == "gatys":
        gatys(rgbfile, stylepng, algfile, iteration, size, weight, stylescale, color, otherparams)
    elif alg == "gatys-multiresolution":
        gatys_multiresolution(rgbfile, stylepng, algfile, size, weight, stylescale, color, otherparams)
    elif alg in ["chen-schmidt", "chen-schmidt-inverse"]:
        chenschmidt(alg, rgbfile, stylepng, algfile, size, stylescale, otherparams)
    # Enforce correct size
    correctshape(algfile, content, size)

    # Recover alpha channel
    correctshape(alphafile, content, size)
    mergealpha(algfile, alphafile, outfile)


def neuraltile(content, style, outfile, size, tilesize, tileoverlap, alg, iteration, weight, stylescale, color, otherparams):
    """Strategy to generate a high resolution image by running style transfer on overlapping image tiles"""
    LOGGER.info("Starting tiling strategy")
    if otherparams is None:
        otherparams = []
    workdir = TemporaryDirectory()

    # Gather size info from original image
    fullshape = targetshape(content, size)

    # Compute number of tiles required to map all the image
    xtiles, ytiles = tilegeometry(fullshape, alg, tilesize, tileoverlap)

    # First scale image to target resolution
    firstpass = workdir.name + "/" + "lowres.png"
    convert(content, firstpass)
    resize(firstpass, fullshape)

    # Chop the styled image into tiles with the specified tileoverlap value.
    lowrestiles = choptiles(firstpass, xtiles=xtiles, ytiles=ytiles, tileoverlap=tileoverlap, outname=workdir.name + "/" + "lowres_tiles")

    # High resolution pass over each tile
    highrestiles = []
    for i, tile in enumerate(lowrestiles):
        name = workdir.name + "/" + "highres_tiles_" + str(i) + ".png"
        styletransfer_single(tile, style, name, None, alg, iteration, weight, stylescale, color, otherparams)
        highrestiles.append(name)

    # Feather tiles
    featheredtiles = []
    for i, tile in enumerate(highrestiles):
        name = workdir.name + "/" + "feathered_tiles_" + str(i) + ".png"
        feather(tile, name)
        featheredtiles.append(name)

    # Smush the feathered tiles together
    smushedfeathered = workdir.name + "/" + "feathered_smushed.png"
    smush(featheredtiles, xtiles, ytiles, tileoverlap, tileoverlap, smushedfeathered)

    # Smush also the non-feathered tiles
    smushedhighres = workdir.name + "/" + "highres_smushed.png"
    smush(highrestiles, xtiles, ytiles, tileoverlap, tileoverlap, smushedhighres)

    # Combine feathered and un-feathered output images to disguise feathering
    composite([smushedfeathered, smushedhighres], outfile)

    # Adjust back to desired size
    assertshape(outfile, fullshape)


def gatys(content, style, outfile, iteration, size, weight, stylescale, color, otherparams):
    """Runs Gatys et al style-transfer algorithm

    References:
        * https://arxiv.org/abs/1508.06576
        * https://github.com/jcjohnson/neural-style
    """
    # Gatys can only process one combination of content, style, weight and scale at a time, so we need to iterate
    tmpout = NamedTemporaryFile(suffix=".png")
    runalgorithm("gatys", [
        "-content_image", content,
        "-style_image", style,
        "-num_iterations", iteration,
        "-style_weight", weight * 100,  # Because content weight is 100
        "-style_scale", stylescale,
        "-output_image", tmpout.name,
        "-image_size", size if size is not None else shape(content)[0],
        "-original_colors", color,
        *otherparams
    ])
    # Transform to original file format
    convert(tmpout.name, outfile)
    tmpout.close()


def gatys_multiresolution(content, style, outfile, size, weight, stylescale, otherparams, startres=256):
    """Runs a multiresolution version of Gatys et al method

    The multiresolution strategy starts by generating a small image, then using that image as initializer
    for higher resolution images. This procedure is repeated up to the tilesize.

    Once the maximum tile size attainable by L-BFGS is reached, more iterations are run by using Adam. This allows
    to produce larger images using this method than the basic Gatys.

    References:
        * Gatys et al - Controlling Perceptual Factors in Neural Style Transfer (https://arxiv.org/abs/1611.07865)
        * https://gist.github.com/jcjohnson/ca1f29057a187bc7721a3a8c418cc7db
    """
    # Multiresolution strategy: list of rounds, each round composed of a optimization method and a number of
    # upresolution steps.
    # Using "adam" as optimizer means that Adam will be used when necessary to attain higher resolutions
    strategy = [
        ["lbfgs", 7],
        ["lbfgs", 7],
        ["lbfgs", 7],
        ["lbfgs", 7],
        ["lbfgs", 7]
    ]
    LOGGER.info("Starting gatys-multiresolution with strategy " + str(strategy))

    # Initialization
    workdir = TemporaryDirectory()
    maxres = targetshape(content, size)[0]
    if maxres < startres:
        LOGGER.warning("Target resolution (%d) might too small for the multiresolution method to work well" % maxres)
        startres = maxres / 2.0
    seed = None
    tmpout = workdir.name + "/tmpout.png"

    # Iterate over rounds
    for roundnumber, (optimizer, steps) in enumerate(strategy):
        LOGGER.info("gatys-multiresolution round %d with %s optimizer and %d steps" % (roundnumber, optimizer, steps))
        roundmax = min(maxtile(), maxres) if optimizer == "lbfgs" else maxres
        resolutions = np.linspace(startres, roundmax, steps, dtype=int)
        iters = 1000
        for stepnumber, res in enumerate(resolutions):
            stepopt = "adam" if res > maxtile() else "lbfgs"
            LOGGER.info("Step %d, resolution %d, optimizer %s" % (stepnumber, res, stepopt))
            passparams = otherparams[:]
            passparams.extend([
                "-num_iterations", iters,
                "-tv_weight", "0",
                "-print_iter", "0",
                "-optimizer", stepopt
            ])
            if seed is not None:
                passparams.extend([
                    "-init", "image",
                    "-init_image", seed
                ])
            gatys(content, style, tmpout, res, weight, stylescale, color, passparams)
            seed = workdir.name + "/seed.png"
            copyfile(tmpout, seed)
            iters = max(iters/2.0, 100)

    convert(tmpout, outfile)


def chenschmidt(alg, content, style, outfile, size, stylescale, otherparams):
    """Runs Chen and Schmidt fast style-transfer algorithm

    References:
        * https://arxiv.org/pdf/1612.04337.pdf
        * https://github.com/rtqichen/style-swap
    """
    if alg not in ["chen-schmidt", "chen-schmidt-inverse"]:
        raise ValueError("Unnaceptable subalgorithm %s for Chen-Schmidt family")

    # Rescale style as requested
    instyle = NamedTemporaryFile()
    copyfile(style, instyle.name)
    resize(instyle.name, int(stylescale * shape(style)[0]))
    # Run algorithm
    outdir = TemporaryDirectory()
    runalgorithm(alg, [
        "--save", outdir.name,
        "--content", content,
        "--style", instyle.name,
        "--maxContentSize", size if size is not None else shape(content)[0],
        "--maxStyleSize", size if size is not None else shape(content)[0],
        *otherparams
    ])
    # Gather output results
    output = outdir.name + "/" + filename(content) + "_stylized" + fileext(content)
    convert(output, outfile)
    instyle.close()


def runalgorithm(alg, params):
    """Run a style transfer algorithm with given parameters"""
    # Move to algorithm folder
    command = "cd " + ALGORITHMS[alg]["folder"] + "; "
    # Algorithm command with default parameters
    command += ALGORITHMS[alg]["command"] + " " + " ".join(ALGORITHMS[alg]["defaultpars"])
    # Add provided parameters, if any
    command += " " + " ".join([str(p) for p in params])
    #LOGGER.info("Running command: %s" % command)
    call(command, shell=True)


def outname(savefolder, content, style, alg, iteration, size, stylescale, weight, color):
    """Creates an output filename that reflects the style transfer parameters"""
    return (
        savefolder + "/" +
	datetime.datetime.now().strftime('%Y-%m-%d_') +
        filename(content) +
        "_" + filename(style) +
        "_" + str(size) + "px" +
        "_" + alg + "_" + str(iteration) + "x" +
        "_ss_" + str(stylescale) +
        "_sw_" + str(weight) +
        "_color_" + str(color) +
        fileext(content)
    )


def correctshape(result, original, size=None):
    """Corrects the result of style transfer to ensure shape is coherent with original image and desired output size

    If output size is not specified, the result image is corrected to have the same shape as the original.
    """
    assertshape(result, targetshape(original, size))


def tilegeometry(imshape, alg, tilesize, tileoverlap):
    """Given the shape of an image, computes the number of X and Y tiles to cover it"""

    xtiles = ceil(float(imshape[0] - tilesize) / float(tilesize - tileoverlap) + 1)
    ytiles = ceil(float(imshape[1] - tilesize) / float(tilesize - tileoverlap) + 1)
	
    LOGGER.info("number x tiles: %s" % xtiles)
    LOGGER.info("number y tiles: %s" % ytiles)

    return xtiles, ytiles


def fitsingletile(imshape, alg, tilesize):
    """Returns whether a given image shape will fit in a single tile or not. This depends on the algorithm used and the GPU available in the system"""
	
    return tilesize*tilesize >= np.prod(imshape)


def targetshape(content, size=None):
    """Computes the shape the resultant image will have after a reshape of the size given

    If size is None, return original shape.
    """
    contentshape = shape(content)
    if size is None:
        return contentshape
    else:
        return [size, int(size * contentshape[1] / contentshape[0])]

