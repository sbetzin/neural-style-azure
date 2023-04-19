import os
import urllib.request
import argparse

def download_models(model_url_path_list):
    for url, target_path in model_url_path_list:
        target_dir = os.path.dirname(target_path)
        if not os.path.exists(target_dir):
            os.makedirs(target_dir)

        if not os.path.exists(target_path):
            print(f"Downloading model from {url} to {target_path}...")
            urllib.request.urlretrieve(url, target_path)
            print(f"Successfully downloaded to {target_path}.")
        else:
            print(f"Model already exists at {target_path}. Skipping download.")


def download(project_path):
    urls_and_paths = [
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/color-model.pth", f"{project_path}/checkpoints/color-model.pth"),
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/depth-model.pth", f"{project_path}/checkpoints/depth-model.pth"),
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/edge-model.pth", f"{project_path}/checkpoints/edge-model.pth"),
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/latest_net_G.pth", f"{project_path}/BoostingMonocularDepth/pix2pix/checkpoints/mergemodel/latest_net_G.pth"),
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/model.pt", f"{project_path}/BoostingMonocularDepth/midas/model.pt"),
        ("https://neuralstylefiles.blob.core.windows.net/models/3d-photo-inpainting/res101.pth", f"{project_path}/BoostingMonocularDepth/res101.pth"),
    ]

    download_models(urls_and_paths)


def main(project_path):
    download(project_path)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="A script that receives a project path.")
    parser.add_argument("project_path", help="The project path to be passed.")

    args = parser.parse_args()
    main(args.project_path)

