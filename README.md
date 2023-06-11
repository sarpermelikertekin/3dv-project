# [3D Vision Project] Advanced Surgical Planning - Implant Recognition

![HoloLens](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/7399fd8d-d388-4a19-a4f1-42f3ac3cd88f)

The ETH Spin-off CustomSurg specializes in complex joint fracture surgery using modern-day technology.

This project results from ETH’s and CustomSurg’s joint interest and collaboration in terms of 3D Vision with HoloLens and is realized over a span of 3 months as part of the lecture 3D Vision. 

CustomSurg is currently researching and deploying HoloLenses in complex bone feature surgeries in order to support surgeons making difficult decisions prior, as well as during the surgery. 

In the framework of this  project we focus on choosing the correct implant, by recoginizing it in HoloLens, tracing it, detecting the implant’s type and rendering said type with the implant’s bounding box in HoloLens. 

This way, surgeons can visually confirm that the present implant is truly the implant chosen for a particular surgery.

### In our application we present the following pipeline

HoloLens captures a picture from its camera and sends it over TCP to a server to be processed and given as input to YOLOv5. 

When YOLOv5 receives the image, it sends the class and position information back to the HoloLens as a string (multiple objects are supported), where this string will be further processed in order to place the bounding box and highlight the hologram of the detected implant.

![3dv-system](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/e7af21cd-7c23-4621-bb19-9ec15fbc6e20)

### This repo contains following projects 
- 3DV Hololens Main App (Unity App)
- 3DV Off Device ML (Python Backend)
- 3DV Hololens Bounding Box (Unity App)
- 3DV Data Generator (Unity App)
- (Optional) Vuforia integration

## 3DV Hololens Main App (Unity App)

In this project, we present a Unity app that establishes a connection with a python server and sends images captured by hololens to be processed by the YOLOv5. After the process, the results will be received back by the Unity App and the string will be processed further to implement highlighting and bounding box features.

**! Caution: Server should run before the connect button in Hololens App is pressed.**

### Getting Started
The tutorial below will show you how to **Build and deploy the Unity app**.

#### Things you need
* [The HoloLens development tools](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools), sans Vuforia and the Emulator.

#### Build and deploy the Unity app
1. **Open the Hololens Stream Scene:** Navigate to and open the project directory (`../Scenes/Hololens Stream`) in Unity.
2. **Configure build settings:** Once the project opens, select **File > Build Settings**. In the Platform list, select `Windows Store`. Set the SDK to `Universal 10`; set Target device to `HoloLens`; set UWP Build Type to `D3D`; check the Unity C# Projects checkbox; and finally, click **Switch Platform**.
3. **Add Define Symbols:** Open **File > Build Settings > Player Settings > Other Settings** and add the following to `Scripting Define Symbols` depending on the XR system used in your project; Legacy built-in XR: `BUILTIN_XR`'; XR Plugin Management (Windows Mixed Reality): `XR_PLUGIN_WINDOWSMR`; XR Plugin Management (OpenXR):`XR_PLUGIN_OPENXR`.
4. **Build the project:** You can now build the Unity project, which generates a Visual Studio Solution (which you will then have to also build). With the Build Settings window still open, click **Build**. In the explorer window that appears, make a new folder called `App`, which should live as a sibling next to the 'Assets` folder. Then click Select Folder to generate the VS solution in that folder. Then wait for Unity to build the solution.
5. **Open the VS Solution:** When the solution is built, a Windows explorer folder will open. Open the newly-built VS solution, which lives in `App/HoloLensVideoCaptureExample.sln`. This is the solution that ultimately gets deployed to your HoloLens.
6. **Configure the deploy settings:** In the Visual Studio toolbar, change the solution platform from `ARM` to `x86` if you are building for Hololens1 or to `ARM64` if you are building for Hololens2; Change the deploy target (the green play button) to `Device` (if your HoloLens is plugged into your computer), or `Remote Machine` (if your HoloLens is connected via WiFi).
7. **Run the app:** Go to **Debug > Start Without Debugging**. Once the app is deployed to the HoloLens, you should see.

![HighlightedImplant](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/f9ee6bbf-fb99-4f30-8b98-bcfd43ff4e11)

## Off Device ML (Python)

1. **Clone Yolov5:** git clone https://github.com/ultralytics/yolov5 in the root of repository.
2. **Install dependencies:** cd yolov5 / pip install -r requirements.txt
3. **Run `server.py` under PythonBackend:** It runs the server in which the Yolo model is prepared for inference. Then it starts listening images from Hololens. Assuming the IP address of the server and correct port is entered in Hololens, the communication will be established. Then the images are read in chunks and reconstructed in a file. The image in the file is passed to Yolo model, and model returns the list of detected objects and their coordinates and labels. The model response is sent back to Hololens. This loop goes on until the program terminates from Hololens side. 
4. **Customize parameters:** Port number, path of images, and path to the weights being used in Yolo model can be arranged in `server.py`.  

**! Caution: Server should run before the connect button in Hololens App is pressed.**

![train_batch2](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/c91fb5f0-450b-488d-817a-2e0277a7ebe6)
![confusion_matrix](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/b881a70c-ea42-407f-8526-03df5f33d69c)



## HoloLensBoundingBox (Unity App)

This project is based on [**HoloLensCameraStream for Unity**](https://github.com/EnoxSoftware/HoloLensCameraStream). The original repository provides Unity plugin for mapping image pixel coordinate from the HoloLens video camera to 3D coordinate. In this repo, we present a Unity app that 
1. Displays in real-time what the hololens sees, i.e. HoloLens video stream;
2. Converts bounding boxes specified in image pixel coordinate system to HoloLens world coordinate system;
3. Displays the bounding box on top of the HoloLens video stream.

### Getting Started
The tutorial below will show you how to **Build and deploy the Unity app**.

#### Things you need
* [The HoloLens development tools](https://developer.microsoft.com/en-us/windows/mixed-reality/install_the_tools), sans Vuforia and the Emulator.

#### Build and deploy the Unity app
The example Unity project can be found in the root `HoloLensVideoCaptureExample/` directory. This Unity project is a great way to learn how to use the CameraStream plugin in Unity, or to use as a template for your own Unity project. Read on to learn how to build and run the example project on your HoloLens. You should be familiar with creating and configuring a new Unity-HoloLens project [according to Microsoft's instructions](https://developer.microsoft.com/en-us/windows/mixed-reality/holograms_100). As Microsoft and Unity update their HoloLens documentation, I'm sure this tutorial will become out of date.
1. **Open the example project:** Navigate to and open the project directory (`HoloLensBoundingBox/`) in Unity.
2. **Configure build settings:** Once the project opens, select **File > Build Settings**. In the Platform list, select `Windows Store`. Set the SDK to `Universal 10`; set Target device to `HoloLens`; set UWP Build Type to `D3D`; check the Unity C# Projects checkbox; and finally, click **Switch Platform**.
3. **Add Define Symbols:** Open **File > Build Settings > Player Settings > Other Settings** and add the following to `Scripting Define Symbols` depending on the XR system used in your project; Legacy built-in XR: `BUILTIN_XR`'; XR Plugin Management (Windows Mixed Reality): `XR_PLUGIN_WINDOWSMR`; XR Plugin Management (OpenXR):`XR_PLUGIN_OPENXR`.
4. **Build the project:** You can now build the Unity project, which generates a Visual Studio Solution (which you will then have to also build). With the Build Settings window still open, click **Build**. In the explorer window that appears, make a new folder called `App`, which should live as a sibling next to the 'Assets` folder. Then click Select Folder to generate the VS solution in that folder. Then wait for Unity to build the solution.
5. **Open the VS Solution:** When the solution is built, a Windows explorer folder will open. Open the newly-built VS solution, which lives in `App/HoloLensVideoCaptureExample.sln`. This is the solution that ultimately gets deployed to your HoloLens.
6. **Configure the deploy settings:** In the Visual Studio toolbar, change the solution platform from `ARM` to `x86` if you are building for Hololens1 or to `ARM64` if you are building for Hololens2; Change the deploy target (the green play button) to `Device` (if your HoloLens is plugged into your computer), or `Remote Machine` (if your HoloLens is connected via WiFi).
7. **Run the app:** Go to **Debug > Start Without Debugging**. Once the app is deployed to the HoloLens, you should see.


![Screenshot_20230611-162252_WhatsApp](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/b873c8c9-6bde-4ef0-b145-11f4eb910c37)


## 3DV Data Generator (Unity App)

In this project, we present a Unity app that is able to generate synthetic data to finetune YOLOv5

### Generating Data

In the sample scene (../Assets/Scenes/SampleScene) select Controller Gameobject.

Under DatasetGenerator Script:
1. Name the output file 
2. Define the number of images to be generated
3. Define waiting time after each image generated
4. Make sure "Capture" variable under Capture Screenshot is checked
5. Press Play
6. Press A (default by A can be changed under Data Set Generator => Starting Key)

### Important

- You might want to save the images to a folder which is outside of the Unity Projects (Assets Folder), in order to avoid getting the Metadata generated, which takes a lot of time and space (select Save OutsideOf The Application and specify Output File Location)
- Game Panel should be visible all the time so that Camera can capture the screen content (It is okey if it is not visible on the screen, meaning Unity is ocluded by different windows but important part is Game Panel should be visible in Unity)

![image (4)](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/55a123c1-e8a1-4230-9632-eba4782d2d2e)

![synthetic_img](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/c899437a-85d8-4779-b5d8-d2cf310dd884)

## Vuforia Integration

Since this one isn't directly a part of the project but rather will serve as a comparison point for us, we haven't added the assets yet to the repository.

However if we agree with CustomSurg and the TAs we will also provide the code and explanation

![Vuforia](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/49061298-6761-44bd-92cc-1194306eaa3e)

![MTGenerator](https://github.com/sarpermelikertekin/3dv-project-advanced-surgical-planning-implant-recognition/assets/49168444/68ccd592-9140-4f16-b1a4-56afb5b10c81)

