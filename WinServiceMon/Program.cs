using Topshelf;

namespace WinServiceMon
{
    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>                                  
            {
                x.Service<Monitor>(s =>                                   
                {
                    s.ConstructUsing(name => new Monitor());                
                    s.WhenStarted(tc => tc.Start());                        
                    s.WhenStopped(tc => tc.Stop()); 
                                            
                });

                x.UseNLog();

                x.RunAsLocalSystem();

                x.StartAutomatically();                                    

                x.SetDescription("AlertBoy Windows Service to monitor other windows services ");                   
                x.SetDisplayName("AlertBoy ServMon");                                  
                x.SetServiceName("AlertBoy_Servmon");                                 
            });
        }
    }
}
