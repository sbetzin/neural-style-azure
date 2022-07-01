import torch
import torchvision as tv 
import torch.optim as optim
from torchvision import transforms, models 
import numpy as np 
from PIL import Image
import matplotlib.pyplot as plt

mean = np.asarray([ 0.485, 0.456, 0.406 ])
std = np.asarray([ 0.229, 0.224, 0.225 ])


def load_image_as_tensor(img_path, device, max_size=1200, shape=None):        
    image = Image.open(img_path).convert('RGB')
    w, h=image.size[0], image.size[1]
    # print(f'size={image.size}, h={h}, w={w} ')    
    
    # large images will slow down processing    
    if max(image.size) > max_size:
        scale=max_size/max(image.size)
        size = torch.Size((int(h*scale), int (w*scale)))
    else:        
            size=torch.Size((h, w))
            
    if shape is not None:
        size=shape
             
    tfm = transforms.Compose([
        transforms.Resize(size), 
        transforms.ToTensor(),
        # normalize image based on mean and std of ImageNet dataset
        transforms.Normalize(mean, std, inplace=True), 
        transforms.Lambda(lambda x: x.mul(255))
        ])

    # discard the transparent, alpha channel (that's the :3) and add the batch dimension
    return tfm(image)[:3,:,:].unsqueeze(0).to(device) 

def save_image_from_tensor(img, filename):    
    tfm=transforms.Compose([
        # reverse the normalization
        transforms.Lambda(lambda x: x.div(255) ), 
        transforms.Normalize((-1 * mean / std), (1.0 / std),inplace=True) 
        ])
    
    img2=tfm(img.cpu().squeeze(0))

    tv.utils.save_image(img2, filename)
    
    return img2.permute(1, 2, 0).detach().numpy()   