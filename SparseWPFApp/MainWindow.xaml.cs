using DesktopBridge;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using Windows.Management.Deployment;
using Windows.Storage;

namespace SparseWPFApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //// Option 1: Attempt to access ApplicationData
            //try
            //{
            //    var storage = ApplicationData.Current.LocalFolder;
            //    IdentityText.Text = "Has Identity" ;
            //}
            //catch
            //{
            //    IdentityText.Text = "No Identity";
            //}

            //// Option 2: Use the DesktopBridge.Helpers library
            //IdentityText.Text = new Helpers().IsRunningAsUwp() ? "Has Identity" : "No Identity";

            // Option 3: Invoke GetCurrentPackageFullName to get the package name
            IdentityText.Text = ApplicationIdentity is string id ? $"Identity {id}" : "No identity";
        }

        const long APPMODEL_ERROR_NO_PACKAGE = 15700L;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

        public string ApplicationIdentity
        {
            get
            {
                int length = 0;
                StringBuilder sb = new StringBuilder(0);
                int result = GetCurrentPackageFullName(ref length, sb);

                sb = new StringBuilder(length);
                result = GetCurrentPackageFullName(ref length, sb);

                return result != APPMODEL_ERROR_NO_PACKAGE ? sb.ToString() : null;
            }
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            string externalLocation = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            string sparsePkgPath = Path.Combine(externalLocation, "package.msix");

            //Attempt registration
            var result = registerSparsePackage(externalLocation, sparsePkgPath);
            if (result)
            {
                IdentityText.Text = "Registered for Identity - Click Restart";
            }
            else
            {
                IdentityText.Text = "Unable to register";
            }
        }

        private void UnregisterClick(object sender, RoutedEventArgs e)
        {
            removeSparsePackage();
            IdentityText.Text = "Unregistered - Click Restart";
        }

        private void RestartClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();

        }


        private static bool registerSparsePackage(string externalLocation, string sparsePkgPath)
        {
            bool registration = false;
            try
            {
                Uri externalUri = new Uri(externalLocation);
                Uri packageUri = new Uri(sparsePkgPath);

                Console.WriteLine("exe Location {0}", externalLocation);
                Console.WriteLine("msix Address {0}", sparsePkgPath);

                Console.WriteLine("  exe Uri {0}", externalUri);
                Console.WriteLine("  msix Uri {0}", packageUri);

                PackageManager packageManager = new PackageManager();

                //Declare use of an external location
                var options = new AddPackageOptions();
                options.ExternalLocationUri = externalUri;

                Windows.Foundation.IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = packageManager.AddPackageByUriAsync(packageUri, options);

                ManualResetEvent opCompletedEvent = new ManualResetEvent(false); // this event will be signaled when the deployment operation has completed.

                deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };

                Console.WriteLine("Installing package {0}", sparsePkgPath);

                Debug.WriteLine("Waiting for package registration to complete...");

                opCompletedEvent.WaitOne();

                if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Error)
                {
                    Windows.Management.Deployment.DeploymentResult deploymentResult = deploymentOperation.GetResults();
                    Debug.WriteLine("Installation Error: {0}", deploymentOperation.ErrorCode);
                    Debug.WriteLine("Detailed Error Text: {0}", deploymentResult.ErrorText);

                }
                else if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Canceled)
                {
                    Debug.WriteLine("Package Registration Canceled");
                }
                else if (deploymentOperation.Status == Windows.Foundation.AsyncStatus.Completed)
                {
                    registration = true;
                    Debug.WriteLine("Package Registration succeeded!");
                }
                else
                {
                    Debug.WriteLine("Installation status unknown");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddPackageSample failed, error message: {0}", ex.Message);
                Console.WriteLine("Full Stacktrace: {0}", ex.ToString());

                return registration;
            }

            return registration;
        }

        private void removeSparsePackage()
        {
            PackageManager packageManager = new PackageManager();
            Windows.Foundation.IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = packageManager.RemovePackageAsync(ApplicationIdentity);
            ManualResetEvent opCompletedEvent = new ManualResetEvent(false); // this event will be signaled when the deployment operation has completed.

            deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };

            Debug.WriteLine("Uninstalling package..");
            opCompletedEvent.WaitOne();
        }
    }
}
