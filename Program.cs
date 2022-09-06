/*
 * Reference:
 * 1. ServiceController API - https://docs.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller?view=dotnet-plat-ext-6.0
 * 2. Task Delay API - https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay?view=net-6.0
 * 3. DateTime API - https://docs.microsoft.com/en-us/dotnet/api/system.datetime?view=net-6.0
 * 4. TopShelf - https://www.c-sharpcorner.com/article/creating-windows-service-in-net-with-topshelf/
 * 
 * Framework
 * 1. You can use .NET6/.NET5 or .NET Framework
 * 
 * Nuget Package
 * 1. Need to install "System.ServiceProcess.ServiceController" package.
 * 
 * If you want to this appllication change to windows services, please add 'TopShelf' package.
 * 
*/


using System.Diagnostics;
using System.ServiceProcess;

// Use LINQ to Get All Devices
//var allServices = ServiceController.GetDevices().Select(s => s);  // First method
var allServices = ServiceController.GetDevices().ToList();          // Second method


Console.WriteLine("All services has {0}",allServices.Count());
//var fw = File.AppendText(@"./services.txt");

foreach (var ser in allServices)    
{
    //fw.WriteLine("{0}:\t{1}", ser.ServiceName, ser.Status);
    //Console.WriteLine("{0}:\t{1}", ser.ServiceName,ser.Status);
}

//fw.Close();

var now = DateTime.Now;
// 0000 restart = 06
// 0004 +1 = 07
//var settingTime = new DateTime(now.Year, now.Month, (now.Day + 1), 0, 4, 0);
var settingTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, (now.Second + 10));

var differSecs = (settingTime - now).TotalSeconds;

Console.WriteLine("Current time: {0}", now.ToString());
Console.WriteLine("Setting time: {0}", settingTime.ToString());
Console.WriteLine("Different of total seconds: {0}", differSecs);

//Task delay = asyncTask(Convert.ToInt32(differSecs));
//delay.Wait();

Task startService = StartServices(Convert.ToInt32(differSecs));
startService.Wait();

Task startService2 = StartServices(Convert.ToInt32(differSecs));
startService2.Wait();
//StartServices();


async Task asyncTask(int delayTime) {

    // Sleep the Main thread for 1 seconds
    //Thread.Sleep(5000);
    var sw = new Stopwatch();
    sw.Start();
    Console.WriteLine("Running milliseconds: {0}", sw.ElapsedMilliseconds);
    await Task.Delay(delayTime* 1000); 
    Console.WriteLine("Running milliseconds: {0}", sw.ElapsedMilliseconds);
    await Task.Delay(3000);
    Console.WriteLine("Running milliseconds: {0}", sw.ElapsedMilliseconds);
}

//void StartServices()
async Task StartServices(int delayTime)
{
    // Enter the services you want to start
    string selectServiceName = "XboxNetApiSvc";
    await Task.Delay(delayTime * 1000);
    // LINQ 
    var selectedService = ServiceController.GetServices().Where(s => selectServiceName.Contains(s.ServiceName));
    Console.WriteLine("Selecte Service Count: {0}",selectedService.Count());
    foreach(var service in selectedService)
    {
        Console.WriteLine("Services Name: {0}", service.ServiceName);
        Console.WriteLine("Services Status: {0}", service.Status);  
        Console.WriteLine("CanStop: {0}", service.CanStop);

        for(int i =0;i <10; i++)
        {
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
                // Waiting for Windows start the service, it may take some more than 1 seconds
                // Depend on your services size.
                Thread.Sleep(1000);
                service.Refresh();
                Console.WriteLine("Services Status after Start: {0}", service.Status);
                break;
            }
            else if(service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();               
                // Waiting for Windows stop the service, it may take some more than 1 seconds
                Thread.Sleep(1000); 
                service.Refresh();
                Console.WriteLine("Services Status after Stop: {0}", service.Status);
            }
        }      
    }
}




