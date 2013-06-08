﻿using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Media.Media3D;
using MoveFramework_CS;

using VrPlayer.Contracts.Trackers;

namespace VrPlayer.Trackers.PsMoveTracker
{
    [Export(typeof(ITracker))]
    public class PsMoveTracker : TrackerBase, ITracker
    {
        public PsMoveTracker()
        {
            try
            {
                IsEnabled = true;
                PositionScaleFactor = 0.01;
                Init();
            }
            catch (Exception exc)
            {
                IsEnabled = false;
            }  
        }

        private void Init()
        {
            MoveWrapper.init();
            var moveCount = MoveWrapper.getMovesCount();
            if (moveCount <= 0)
            {
                IsEnabled = false; 
                return;
            }
                
            MoveWrapper.setRumble(0, 255);
            Thread.Sleep(40);
            MoveWrapper.setRumble(0, 0);

            MoveWrapper.subscribeMoveUpdate(
                MoveUpdateCallback,
                MoveKeyDownCallback,
                MoveKeyUpCallback,
                NavUpdateCallback,
                NavKeyDownCallback,
                NavKeyUpCallback
                );
        }

        void MoveUpdateCallback(int id, MoveWrapper.Vector3 pos, MoveWrapper.Quaternion rot, int trigger)
        {
            RawPosition = new Vector3D(pos.x, pos.y, pos.z);
            RawRotation = new Quaternion(rot.x, -rot.y, rot.z, -rot.w);

            if (MoveWrapper.getButtonState(0, MoveButton.B_START))
            {
                Dispatcher.Invoke((Action)(Calibrate));
            }

            Dispatcher.Invoke((Action)(UpdatePositionAndRotation));
        }

    	void MoveKeyUpCallback(int id, int keyCode)
        {
        }

    	void MoveKeyDownCallback(int id, int keyCode)
        {
        }

    	void NavUpdateCallback(int id, int trigger1, int trigger2, int stickX, int stickY)
        {
        }

    	void NavKeyUpCallback(int id, int keyCode)
        {
        }

    	void NavKeyDownCallback(int id, int keyCode)
        {
        }

        public override void Dispose()
        {
            try
            {
                MoveWrapper.unsubscribeMove();
            }
            catch
            {
            }
        }
    }
}
