using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.ServiceProcess;

namespace WinServiceMon
{
   public  class Monitor
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        Timer _timerMonitor;
        Timer _timerResetQueue;

        Queue<AlertMessage> _recentAlerts;

        public Monitor()
        {
            _recentAlerts = new Queue<AlertMessage>();
        }

        public void Start()
        {
            //init Timers
            InitializeTimers();

            //Get Configs
            var installedService = GetInstalledServices();
            if (installedService.Any())
            {
                logger.Info("Found {0} services to monotor.", installedService.Length);
            }
            else
            {
                logger.Info("No services found to monotor.");
            }

            //Start Timers
            _timerMonitor.Start();
            _timerResetQueue.Start();

            logger.Info("Enigma ServMon Service started.");
        }

        public void Stop()
        {
            logger.Warn("Enigma ServMon stopped.");
        }

        private void _timerMonitor_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //We do monitoring
                Alert();
            }
            catch(Exception ex)
            {
                logger.Info("Failed to monitor. Ex" + ex.Message);
            }
            finally
            {
                _timerMonitor.Enabled = true;
            }

            
        }

        private void _timerResetQueue_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Clear alerts Queue
            _recentAlerts.Clear();

            _timerResetQueue.Enabled = true;
        }

        #region Helpers
        private void InitializeTimers()
        {
            //Timer Monitor
            _timerMonitor = new Timer();
            _timerMonitor.Interval = int.Parse(ConfigurationManager.AppSettings["TimerMonitorInterval"]);
            _timerMonitor.AutoReset = false;
            _timerMonitor.Elapsed += _timerMonitor_Elapsed;


            //Timer Queue Reset
            _timerResetQueue = new Timer();
            _timerResetQueue.Interval = int.Parse(ConfigurationManager.AppSettings["TimerResetQueueInterval"]);
            _timerResetQueue.AutoReset = false;
            _timerResetQueue.Elapsed += _timerResetQueue_Elapsed;
        }

        private ServiceController[] GetServicesToMonitor()
        {
            var allServices = ServiceController.GetServices();

            var installedServices = GetInstalledServices();

            return (allServices.Where(serv => installedServices.Contains(serv.ServiceName)).ToArray());
        }

        private string[] GetInstalledServices()
        {
            var config = ConfigurationManager.AppSettings["InstalledServices"];

            return config.Split(',');
        }

        private void Alert()
        {
            var servicesToMonitor = GetServicesToMonitor().ToList();

            servicesToMonitor.ForEach(serv => {

                //get current status
                var currStatus = serv.Status;
                var level = GetLevelByStatus(currStatus);
                var statusMessage = GeLogMessageByStatus(serv);

                //check if recently reported
                var alert = new AlertMessage()
                {
                    ServiceName = serv.ServiceName,
                    Level = level,
                    Message = statusMessage,
                };

                if (!_recentAlerts.Any(a => a == alert))
                {
                    // //Queue
                    _recentAlerts.Enqueue(alert);

                    // Alert
                    logger.Log(level, statusMessage);
                }

            });
        }

        private string GeLogMessageByStatus(ServiceController service)
        {
            return string.Format("{0} has {1}", service.ServiceName, service.Status);
        }

        private LogLevel GetLevelByStatus(ServiceControllerStatus currStatus)
        {
            switch (currStatus)
            {
                case ServiceControllerStatus.Running:
                    return LogLevel.Info;
                case ServiceControllerStatus.Paused:
                    return LogLevel.Warn;
                case ServiceControllerStatus.Stopped:
                    return LogLevel.Error;
                default:
                    return LogLevel.Info;
            }
        }

        #endregion

    }
}
