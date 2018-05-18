# Main manuel entrypoint script to the neural-style app
import sys
import traceback
import logging
from neuralstyle.algorithms import styletransfer
from neuralstyle.utils import sublist

logging.basicConfig(level=logging.INFO)
LOGGER = logging.getLogger(__name__)

HELP = """
neural-style-docker: artistic style between images

    --content CONTENT_IMAGES: file or files with the content images to use
    --style STYLE_IMAGES: file or files with the styles to transfer
    --output OUTPUT_FOLDER: name of the output folder in which to save results
    --size SIZE: size of the output image. Default: content image size
    --sw STYLE_WEIGHT (default 5): weight or list of weights of the style over the content, in range (0, inf)
    --ss STYLE_SCALE (default 1.0): scaling or list of scaling factors for the style images
    --alg ALGORITHM: style-transfer algorithm to use. Must be one of the following:
        gatys                   Highly detailed transfer, slow processing times (default)
        gatys-multiresolution   Multipass version of Gatys method, provides even better quality
        chen-schmidt            Fast patch-based style transfer
        chen-schmidt-inverse    Even faster aproximation to chen-schmidt through the use of an inverse network
    --tilesize TILE_SIZE: tile size to calculate tiling strategy
    --tileoverlap TILE_OVERLAP: overlap of tiles in the style transfer, measured in pixels. If you experience
        artifacts in the image you should try increasing this. Default: 100
    --colors COLORS (0=style color,1=original color): to keep original color or enter 0 1 to do both images
    --iterations ITERATIONS: iteration or list of iterations
    Additionally provided parameters are carried on to the underlying algorithm.
    
"""


def main(argv=None):
    if argv is None:
        argv = sys.argv
    try:
        # Default parameters
        contents = []
        styles = []
        savefolder = "/images"
        size = None
        alg = "gatys"
        weights = None
       	stylescales = None
        colors = None
       	tileoverlap = None
        tilesize = None
        iterations = None
        otherparams = []

        # Gather parameters
        i = 1
        while i < len(argv):
            # References to inputs/outputs are re-referenced to the mounted /images directory
            if argv[i] == "--content":
                contents = ["/images/" + x for x in sublist(argv[i+1:], stopper="-")]
                i += len(contents) + 1
            elif argv[i] == "--style":
                styles = ["/images/" + x for x in sublist(argv[i+1:], stopper="-")]
                i += len(styles) + 1
            # Other general parameters
            elif argv[i] == "--output":
                savefolder = "/images/" + argv[i+1]
                i += 2
            elif argv[i] == "--alg":
                alg = argv[i+1]
                i += 2
            elif argv[i] == "--size":
                size = int(argv[i+1])
                i += 2
            elif argv[i] == "--sw":
                weights = [float(x) for x in sublist(argv[i+1:], stopper="-")]
                i += len(weights) + 1
            elif argv[i] == "--ss":
                stylescales = [float(x) for x in sublist(argv[i+1:], stopper="-")]
                i += len(stylescales) + 1
            elif argv[i] == "--colors":
                colors = [int(x) for x in sublist(argv[i+1:], stopper="-")]
                i += len(colors) + 1
            elif argv[i] == "--tileoverlap":
                tileoverlap = int(argv[i+1])
                i += 2
            elif argv[i] == "--tilesize":
                tilesize = int(argv[i+1])
                i += 2
            elif argv[i] == "--iterations":
                iterations = [int(x) for x in sublist(argv[i+1:], stopper="-")]
                i += len(iterations) + 1
            # Help
            elif argv[i] == "--help":
                print(HELP)
                return 0
            # Additional parameters will be passed on to the specific algorithms
            else:
                otherparams.append(argv[i])
                i += 1

        # Check parameters
        if len(contents) == 0:
            raise ValueError("At least one content image must be provided")
        if len(styles) == 0:
            raise ValueError("At least one style image must be provided")

        LOGGER.info("Starting...")
        styletransfer(contents, styles, savefolder, size, alg, iterations, weights, stylescales, tilesize, tileoverlap, colors, otherparams)

        return 1

    except Exception:
        print(HELP)
        traceback.print_exc()
        return 0


if __name__ == "__main__":
    sys.exit(main())
