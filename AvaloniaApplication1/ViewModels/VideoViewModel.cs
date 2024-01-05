using Avalonia.Threading;
using LibMpv.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModels
{
    public class VideoViewModel : BaseMpvContextViewModel
    {
        public void Play(string rtspUrl)
        {
            try
            {
                Console.WriteLine($"LoadFile");
                base.LoadFile(rtspUrl);
                Console.WriteLine($"Play");
                base.Play();
                Console.WriteLine($"Play end");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override void InvokeInUIThread(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }
}