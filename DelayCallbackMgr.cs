using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.src.Common
{
    public class InvokeManager
    {
        private InvokeManager()    //泛型单件
        {
            timeDelayLst = new List<float>();
            timeStateLst = new List<bool>();
            callbackDelayLst = new List<Action>();
        }

        private IList<float> timeDelayLst;//存延时时间
        private IList<bool> timeStateLst;//存回调状态 --true回调了，false未回调
        private IList<Action> callbackDelayLst;//存回调

        //开启
        public void Invoke(float _timeDelay, Action _callback)
        {
            timeDelayLst.Add(_timeDelay);
            timeStateLst.Add(false);
            callbackDelayLst.Add(_callback);
        }
        //关闭
        public int CancelInvoke(Action _callback, bool _cancelAll = false)    //false：仅取消最后注册的（和延迟时间无关）；返回int型，可以根据剩余的次数立即执行未执行的回调函数
        {
            int tCount = 0;
            for (int i = callbackDelayLst.Count - 1; i >= 0; i--)
            {
                if (callbackDelayLst[i].Equals(_callback))
                {
                    tCount++;
                    timeDelayLst.RemoveAt(i);
                    timeStateLst.RemoveAt(i);
                    callbackDelayLst.RemoveAt(i);
                    if (!_cancelAll)
                    {
                        return tCount;
                    }
                }
            }
            return tCount;
        }
        public void ClearInvoke()
        {
            timeDelayLst.Clear();
            timeStateLst.Clear();
            callbackDelayLst.Clear();
        }
        public void TickUpdate(float _deltaTime)
        {
            filtTick(_deltaTime);
            for (int i = timeStateLst.Count - 1; i >= 0; i--)
            {
                timeStateLst[i] = false;
            }
        }
        private void filtTick(float _deltaTime)
        {
            for (int i = timeDelayLst.Count - 1; i >= 0; i--)
            {
                if (timeStateLst.Count > i && !timeStateLst[i])
                {
                    timeDelayLst[i] -= _deltaTime;
                    timeStateLst[i] = true;
                    if (timeDelayLst[i] <= 0)
                    {
                        var tCallback = callbackDelayLst[i];//从后向前遍历：删除最后注册（和延迟时间无关）的事件回调（可以自行扩展根据延迟时间进行删除）    
                        timeDelayLst.RemoveAt(i);
                        timeStateLst.RemoveAt(i);
                        callbackDelayLst.RemoveAt(i);

                        tCallback();
                        filtTick(_deltaTime);
                    }
                }
            }
        }
    }
}
