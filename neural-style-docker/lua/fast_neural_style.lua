require 'torch'
require 'nn'
require 'image'

require 'fast_neural_style.ShaveImage'
require 'fast_neural_style.TotalVariation'
require 'fast_neural_style.InstanceNormalization'
local utils = require 'fast_neural_style.utils'
local preprocess = require 'fast_neural_style.preprocess'


--[[
Use a trained feedforward model to stylize either a single image or an entire
directory of images.
--]]

local cmd = torch.CmdLine()

-- Model options
cmd:option('-model', 'models/instance_norm/candy.t7')
cmd:option('-image_size', 768)
cmd:option('-median_filter', 3)
cmd:option('-timing', 0)
cmd:option('-original_colors', 1)

-- Input / output options
cmd:option('-input_image', 'images/content/chicago.jpg')
cmd:option('-output_image', 'out.png')
cmd:option('-input_dir', '')
cmd:option('-output_dir', '')

-- GPU options
cmd:option('-gpu', -1)
cmd:option('-backend', 'cuda')
cmd:option('-use_cudnn', 1)
cmd:option('-cudnn_benchmark', 0)


local function original_colors(content, generated)
  local generated_y = image.rgb2yuv(generated)[{{1, 1}}]
  local content_uv = image.rgb2yuv(content)[{{2, 3}}]

  return image.yuv2rgb(torch.cat(generated_y, content_uv, 1))
end

local function print_images_sizes(img, text)
  local h, w = img:size(2), img:size(3)

  print(string.format("image %s h=%d, w=%d", text, h, w))
end

local function main()
  local opt = cmd:parse(arg)

  if (opt.input_image == '') and (opt.input_dir == '') then
    error('Must give exactly one of -input_image or -input_dir')
  end

  local dtype, use_cudnn = utils.setup_gpu(opt.gpu, opt.backend, opt.use_cudnn == 1)
  local ok, checkpoint = pcall(function() return torch.load(opt.model) end)
  if not ok then
    print('ERROR: Could not load model from ' .. opt.model)
    print('You may need to download the pretrained models by running')
    print('bash models/download_style_transfer_models.sh')
    return
  end
  local model = checkpoint.model
  model:evaluate()
  model:type(dtype)
  if use_cudnn then
    cudnn.convert(model, cudnn)
    if opt.cudnn_benchmark == 0 then
      cudnn.benchmark = false
      cudnn.fastest = true
    end
  end

  local preprocess_method = checkpoint.opt.preprocessing or 'vgg'
  local preprocess = preprocess[preprocess_method]

  local function run_image(in_path, out_path)
    local img = image.load(in_path, 3)
    if opt.image_size > 0 then
      img = image.scale(img, opt.image_size)
    end
    local H, W = img:size(2), img:size(3)
    print_images_sizes(img, in_path)

    local img_pre = preprocess.preprocess(img:view(1, 3, H, W)):type(dtype)

    local timer = nil
    if opt.timing == 1 then
      -- Do an extra forward pass to warm up memory and cuDNN
      model:forward(img_pre)
      timer = torch.Timer()
      if cutorch then cutorch.synchronize() end
    end
    local img_out = model:forward(img_pre)

    if opt.timing == 1 then
      if cutorch then cutorch.synchronize() end
      local time = timer:time().real
      print(string.format('Image %s (%d x %d) took %f', in_path, H, W, time))
    end
    local img_out = preprocess.deprocess(img_out)[1]
    print_images_sizes(img_out, "out deprocess")

    local out_dir = paths.dirname(out_path)
    if not path.isdir(out_dir) then
      paths.mkdir(out_dir)
    end

    if opt.median_filter > 0 then
      img_out = utils.median_filter(img_out, opt.median_filter)
      print_images_sizes(img_out, "out median")
    end

    print('Writing output image to ' .. out_path)
    image.save(out_path, img_out)

    if opt.original_colors == 1 then
      print "applying original colors"
      img_out = image.load(out_path, 3)
      img_out = image.scale(img_out, W, H)
      img_out = original_colors(img, img_out)

      image.save(out_path, img_out)
    end
  end


  if opt.input_dir ~= '' then
    if opt.output_dir == '' then
      error('Must give -output_dir with -input_dir')
    end
    for fn in paths.files(opt.input_dir) do
      if utils.is_image_file(fn) then
        local in_path = paths.concat(opt.input_dir, fn)
        local out_path = paths.concat(opt.output_dir, fn)
        run_image(in_path, out_path)
      end
    end
  elseif opt.input_image ~= '' then
    if opt.output_image == '' then
      error('Must give -output_image with -input_image')
    end
    run_image(opt.input_image, opt.output_image)
  end
end


main()
