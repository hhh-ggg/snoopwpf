﻿namespace Snoop
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;
    using Snoop.Data;
    using Snoop.Infrastructure;
    using Snoop.Properties;

    public class ProcessInfo
    {
        private bool? isOwningProcessElevated;

        public ProcessInfo(int processId)
            : this(Process.GetProcessById(processId))
        {
        }

        public ProcessInfo(Process process)
        {
            this.Process = process;
        }

        public Process Process { get; }

        public bool IsProcessElevated => this.isOwningProcessElevated ??= NativeMethods.IsProcessElevated(this.Process);

        public AttachResult Snoop(IntPtr targetHwnd)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                InjectorLauncherManager.Launch(this, targetHwnd, typeof(SnoopManager).GetMethod(nameof(SnoopManager.StartSnoop)), CreateTransientSettingsData(SnoopStartTarget.SnoopUI, targetHwnd));
            }
            catch (Exception e)
            {
                return new AttachResult(e);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            return new AttachResult();
        }

        public AttachResult Magnify(IntPtr targetHwnd)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                InjectorLauncherManager.Launch(this, targetHwnd, typeof(SnoopManager).GetMethod(nameof(SnoopManager.StartSnoop)), CreateTransientSettingsData(SnoopStartTarget.Zoomer, targetHwnd));
            }
            catch (Exception e)
            {
                return new AttachResult(e);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            return new AttachResult();
        }

        private static TransientSettingsData CreateTransientSettingsData(SnoopStartTarget startTarget, IntPtr targetWindowHandle)
        {
            var settings = Settings.Default;

            return new TransientSettingsData
            {
                StartTarget = startTarget,
                TargetWindowHandle = targetWindowHandle.ToInt64(),

                MultipleAppDomainMode = settings.MultipleAppDomainMode,
                MultipleDispatcherMode = settings.MultipleDispatcherMode,
                SetOwnerWindow = settings.SetOwnerWindow,
                ILSpyPath = settings.ILSpyPath
            };
        }
    }
}