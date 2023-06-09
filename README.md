# 3dv-tcp-communication 

This repo contains following projects
- 3DV Data Generator (Unity app)
- Communication over TCP (Unity app)
- Off Device ML (PythonBackend)
- HoloLensBoundingBox (Unity app)



## 3DV Data Generator (Unity app)

In this project, we present a Unity app that 
TODO

### Generating Data

TODO

### Important

- You might want to save the images to a folder which is outside of the Unity Projects (Assets Folder), in order to avoid getting the Metadata generated, which takes a lot of time and space
- Game Panel should be visible all the time so that Camera can capture the screen content (It is okey if it is not visible on the screen, meaning Unity is ocluded by different windows but important part is Game Panel should be visible in Unity)



## Communication over TCP (Unity app)

In this project, we present a Unity app that 
TODO

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

## Off Device ML (Python)

1. **Run `server.py` under PythonBackend:** It runs the server in which the Yolo model is prepared for inference. Then it starts listening images from Hololens. Assuming the IP address of the server and correct port is entered in Hololens, the communication will be established. Then the images are read in chunks and reconstructed in a file. The image in the file is passed to Yolo model, and model returns the list of detected objects and their coordinates and labels. The model response is sent back to Hololens. This loop goes on until the program terminates from Hololens side. 
2. **Customize parameters:** Port number, path of images, and path to the weights being used in Yolo model can be arranged in `server.py`.  

**! Caution: Server should run before the connect button in Hololens App is pressed.**


## HoloLensBoundingBox (Unity app)

This project is based on [**HoloLensCameraStream for Unity**](https://github.com/EnoxSoftware/HoloLensCameraStream). The original repository provides Unity plugin for mapping image pixel coordinate from the HoloLens video camera to 3D coordinate. In this repo, we present a Unity app that 
1. displays in real-time what the hololens sees, i.e. HoloLens video stream;
2. converts bounding boxes specified in image pixel coordinate system to HoloLens world coordinate system;
3. displays the bounding box on top of the HoloLens video stream.

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
