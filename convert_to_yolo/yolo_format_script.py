import numpy as np
import pandas as pd
import os
import random
import shutil
import glob

def iterateTextfile(imagefile_path, datapoints_path, datasetSize):
    print("Iterating Text File...")

    imagefile = open(imagefile_path, 'r')
    lines = imagefile.readlines()

    px_x = 1600
    px_y = 900
    px_bb = 160
    bb_w = px_bb / px_x
    bb_h = px_bb / px_y

    labels = ['Screw1', 'Screw2', 'Screw3', 'Screw4', 'Screw5']
    added = set()

    # Progress variables
    counter = 0
    total_iterations = len(lines[1:])

    for line in lines[1:]:
        tokens = line.split()
        labelstr = tokens[-1]
        label = -1
        for i in range(len(labels)):
            if labels[i] == labelstr:
                label = str(i)
                break
        index_str = tokens[0]
        index = int(index_str[:-1])
        added.add(index)
        posx = float(tokens[1][1:-1])
        posy = float(tokens[2][:-1])
        posx = (posx + 8) / 16
        posy = (posy + 4.5) / 9
        posy = 1 - posy
        scale = tokens[-2][:-1]
        bb_wi = str(float(scale) * bb_w)
        bb_hi = str(float(scale) * bb_h)

        printline = str(label) + " " + str(posx) + " " + str(posy) + " " + bb_wi + " " + bb_hi + "\n"
        f = open(datapoints_path + "Data Point "+ str(index) + ".txt", "a")
        #f.truncate()
        f.write(printline)
        f.close()

        #Display progress
        if counter % 1000 == 0:
            print("A: Current iteration: {0}, Total iterations: {1}".format(counter, total_iterations))
        counter=counter+1
        
    print("--Done--")
    #---------------------------
    total_iterations = datasetSize
    counter = 0

    for i in range(datasetSize):
        #Display progress
        if counter % 1000 == 0:
            print("B: Current iteration: {0}, Total iterations: {1}".format(counter, total_iterations))
        counter=counter+1

        if i not in added:
            f = open(datapoints_path + "Data Point "+ str(i) + ".txt", "w")
            f.close()

    print("Iterating Text File - DONE")

def cleanup(datapoints_path):
    print("Cleaning old textfiles...")

    folder_path = datapoints_path

    # Get a list of all .txt files in the folder
    txt_files = glob.glob(os.path.join(folder_path, "*.txt"))

    # Iterate over the list and delete each file
    for file_path in txt_files:
        os.remove(file_path)
        #print(f"Deleted file: {file_path}")
    
    print("Cleaning textfiles - DONE")
    return

def trainTestSplit(datapoints_path, targetpath, datasetSize, ratio = 0.85):
    print("Splitting Train/Test...")

    noTrain = int(datasetSize * ratio)
    noVal = datasetSize - noTrain
    valIndex = set(random.sample(range(0, datasetSize), noVal))
    targetTrainImg = targetpath + "images/train/"
    targetValImg = targetpath + "images/val/"
    targetTrainLabel = targetpath + "labels/train/"
    targetValLabel = targetpath + "labels/val/"

    total_iterations = datasetSize
    counter = 0
    for i in range(datasetSize):
        #Display progress
        if counter % 100 == 0:
            print("C: Current iteration: {0}, Total iterations: {1}".format(counter, total_iterations))
        counter=counter+1

        pathImg = "Data Point " + str(i) +".png"
        pathTxt = "Data Point " + str(i) +".txt"
        if i in valIndex:
            shutil.copyfile(datapoints_path + pathImg, targetValImg + pathImg)
            shutil.copyfile(datapoints_path + pathTxt, targetValLabel + pathTxt)
        else:
            shutil.copyfile(datapoints_path + pathImg, targetTrainImg + pathImg)
            shutil.copyfile(datapoints_path + pathTxt, targetTrainLabel + pathTxt)
    
    print("Splitting Train/Test - DONE")
    return
        
print(os.listdir())
relative = './convert_to_yolo/'
imagefile_path = relative + 'imagefile.txt'
datapoints_path = relative + 'Data Points/'
targetpath = relative + 'Data_Points_rdy_for_YOLO/'
datasetSize = 5000
cleanup(datapoints_path)
iterateTextfile(imagefile_path, datapoints_path, datasetSize)
trainTestSplit(datapoints_path, targetpath, datasetSize, 0.85)

#print(os.listdir(folderpath))

