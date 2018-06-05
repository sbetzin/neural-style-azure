require 'torch'
require 'image'

local cmd = torch.CmdLine()
cmd:option('-content_image', '/training/in/Ana.jpg')
cmd:option('-generated_image', '/training/out/Ana.jpg')
cmd:option('-target_image', '/training/out/Ana_original.jpg')

local function main(params)
    local content_image = image.load(params.content_image, 3)
    local generated_image = image.load(params.generated_image, 3)

    local h, w = generated_image:size(2), generated_image:size(3)
    content_image = image.scale(content_image, w, h)

    local corrected_image = original_colors(content_image, generated_image)

    image.save(params.target_image, corrected_image)
end

local function original_colors(content, generated)
    local generated_y = image.rgb2yuv(generated)[{{1, 1}}]
    local content_uv = image.rgb2yuv(content)[{{2, 3}}]
  
    return image.yuv2rgb(torch.cat(generated_y, content_uv, 1))
end

local params = cmd:parse(arg)

main(params)