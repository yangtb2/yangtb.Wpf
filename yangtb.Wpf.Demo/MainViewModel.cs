using System.Threading.Tasks;
using yangtb.Wpf.Common;

namespace yangtb.Wpf.Demo
{
    public class MainViewModel : PropertyNotify
    {
        private AsyncCommand _test = new AsyncCommand((param)=>Task.Delay(3000));
        public AsyncCommand Test => _test;
    }
}