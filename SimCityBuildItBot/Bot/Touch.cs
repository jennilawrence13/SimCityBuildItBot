﻿namespace SimCityBuildItBot.Bot
{
    using Common.Logging;
    using Managed.Adb;
    using System.Collections.Generic;
    using System.Drawing;

    public class Touch
    {
        private ConsoleOutputReceiver creciever = new ConsoleOutputReceiver();
        private Device device;
        private ILog log;

        public Touch(ILog log)
        {
            this.log = log;
            List<Device> devices = AdbHelper.Instance.GetDevices(AndroidDebugBridge.SocketAddress);
            device = devices[0];
        }

        public void TouchDown()
        {
            device.ExecuteShellCommand("sendevent /dev/input/event7 1 330 1", creciever);
            device.ExecuteShellCommand("sendevent /dev/input/event7 3 58 1", creciever);
        }

        public void MoveTo(Point point)
        {
            device.ExecuteShellCommand("sendevent /dev/input/event7 3 53 " + point.X, creciever);
            device.ExecuteShellCommand("sendevent /dev/input/event7 3 54 " + point.Y, creciever);
            EndTouchData();
        }

        public void TouchUp()
        {
            device.ExecuteShellCommand("sendevent /dev/input/event7 1 330 0", creciever);
            device.ExecuteShellCommand("sendevent /dev/input/event7 3 58 0", creciever);
        }

        public void EndTouchData()
        {
            device.ExecuteShellCommand("sendevent /dev/input/event7 0 2 0", creciever);
            device.ExecuteShellCommand("sendevent /dev/input/event7 0 0 0", creciever);
        }

        public void ClickAt(Location location)
        {
            //log.Info("clicking " + location.ToString());

            var point = Constants.GetPoint(location);

            //this.MoveTo(point);

            this.TouchDown();
            this.MoveTo(point);

            this.TouchUp();
            this.EndTouchData();

            System.Threading.Thread.Sleep(1000);
        }

        public void Swipe(Location downAt, Location from, Location to, int steps, bool isDrop)
        {
            var pointdownAt = Constants.GetPoint(downAt);
            var pointFrom = Constants.GetPoint(from);
            var pointTo = Constants.GetPoint(to);

            var xStep = (pointTo.X - pointFrom.X) / steps;
            var yStep = (pointTo.Y - pointFrom.Y) / steps;

            //log.Info("Swiping from" + from.ToString() + " to " + to.ToString());
            TouchDown();
            this.MoveTo(pointdownAt);
            System.Threading.Thread.Sleep(300);
            this.MoveTo(pointFrom);
            System.Threading.Thread.Sleep(300);

            for (int i = 0; i < steps; i++)
            {
                pointFrom.X += xStep;
                pointFrom.Y += yStep;
                this.MoveTo(pointFrom);
            }

            if (!isDrop)
            {
                this.MoveTo(pointdownAt);
            }

            System.Threading.Thread.Sleep(500);
            TouchUp();
            EndTouchData();
            System.Threading.Thread.Sleep(500);
        }
    }
}