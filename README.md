# TrampzSDK
Tramps SDK is a simple API for the buttons in the [TI-BLE-Sensortag](http://www.ti.com/tool/cc2541dk-sensor) that makes it easy to define callbacks for the different button presses on the device without any required knowledge of the Bluetooth Low Energy (BLE) stack. 

In case you are not interested in forking this repository or building your own dll you can download the ready to use library  [here](https://www.nada.kth.se/~ezeddin/trampzsdk/TrampzSDK.dll).

## Documentation
Complete documentation for this SDK can be found [here](https://www.nada.kth.se/~ezeddin/trampzsdk/annotated.html).

## How to Hello Trampz (Tutorial)
1. Open **VisualStudio**
2. __FILE -> New -> Project -> Console Application__.
3. Right click __references -> Add references__
4. Click the __Browse__ and navigate to [TrampzSDK.dll](https://www.nada.kth.se/~ezeddin/trampzsdk/TrampzSDK.dll) and then click the __Add__ button.
5. Create a new listener class to define button behaviour:

      ``` csharp
      class MyListener : BLEButtonListener 
      {
          public override void OnLeft(BLEButton sender, DateTimeOffset timestamp)
          {
              Console.WriteLine("Hello TrampzSDK!");
          }
      }
      ```
      
5. In the imports of your classes add: `using TrampzSDK;` 
6. In`Main` write: 

    ``` csharp
      public static void Main(string[] args)
      {
        //Create a factory and ask for all BLE buttons there are.
        BLEButtonFactory factory = new BLEButtonFactory();
        List<BLEButton> allButtons = factory.GetAllButtons();
    
        //Program and connect all button.
        foreach(BLEButton button in allButtons)
        {
            Console.WriteLine("Found a button!");
            button.Listener = new MyListener();
            button.Connect();
        }
        //Wait until user presses enter before closing console.
        Console.ReadLine(); 
      }
    ```
    
7. Done. Now you can start the application. It will print the message every time you press the left button on a coupled sensortag.
    
## Windows 8 and BLE (Important, specially if you have never used BLE with Windows)
Technically speaking you should not have to pair your BLE devices in order to communicate with them. However that is **not** the case with the Windows 8 operating system. Much like traditional Bluetooth you will have to couple your BLE device with your operating system. To do this follow these steps:
    
1. Press **WindowsKey + C**, to show the charms bar.
2. Press on **Settings**->**Change PC Settings**->**PC & Devices**->**Bluetooth**
3. Select the **SensorTag** and press **Pair**
4. It will ask for a password. Enter `0000`
5. Turn on your TI-sensortag by pressing the button on the side.
6. Your TI-sensortag is now ready to use.
    
