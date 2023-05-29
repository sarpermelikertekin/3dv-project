import os

index = 0
root = "./Dataset_3DVision/real_img_dataset/"

changed = set()

for filename in os.listdir("./Dataset_3DVision/real_img_dataset"):
    
    if filename[:3] != 'ima':
        continue
    
    name = filename[:-4]
    if name in changed:
        continue
    changed.add(name)

    path = root + filename
    source_img = root + name + ".png"
    source_txt = root + name + ".txt"
    target_img = root + "images" + str(index) + ".png"
    target_txt = root + "images" + str(index) + ".txt"
    os.rename(source_img, target_img)
    os.rename(source_txt, target_txt)
    index += 1

