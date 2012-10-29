using System.Runtime.CompilerServices;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Touch;

namespace MFE.Graphics.Native
{
    //public enum InputDeviceType
    //{
    //    Button = 0,
    //    Touch = 1,
    //    Generic = 2,
    //    Last = 3,
    //}

    enum TouchMessages : byte
    {
        Down = 1,
        Up = 2,
        Move = 3,
    }

    class TouchEventListener : IEventListener
    {
        //private static InputManager inputManager = null;
        //public DeviceEvents[] InputDeviceEvents;

        //public TouchEventListener()
        //{
        //    for (int i = 0; i < (int)InputDeviceType.Last; i++)
        //        InputDeviceEvents[i] = new DeviceEvents();
        //}


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InitializeForEventSource()
        {
            //if (inputManager == null)
            //    inputManager = InputManager.CurrentInputManager;
        }
        public bool OnEvent(BaseEvent e)
        {
            //InputReport inputReport = null;
            //InputDevice inputDevice = null;

            if (e is TouchEvent)
            {
                TouchEvent touchEvent = e as TouchEvent;
                //inputManager.TouchDevice.SetTarget(targetWindow != null ? targetWindow : MainWindow);
                //inputReport = new RawTouchInputReport(null, touchEvent.Time, touchEvent.EventMessage, touchEvent.Touches);
                //inputDevice = inputManager.TouchDevice;


                if (touchEvent.EventMessage == (byte)TouchMessages.Down)
                {
                    int a = 0;
                }
                else if (touchEvent.EventMessage == (byte)TouchMessages.Up)
                {
                    int a = 0;
                }
                else if (touchEvent.EventMessage == (byte)TouchMessages.Move)
                {
                    int a = 0;
                }
            }
            else if (e is GenericEvent)
            {
                GenericEvent genericEvent = e as GenericEvent;
                //inputManager.GenericDevice.SetTarget(targetWindow != null ? targetWindow : MainWindow);
                //inputReport = new RawGenericInputReport(null, genericEvent);
                //inputDevice = inputManager.GenericDevice;

                switch (genericEvent.EventMessage)
                {
                    case (byte)TouchGesture.Up:
                        int a = 0;
                        break;

                }


            }
            else
            {
                // Unknown event.
            }

            //if (inputReport != null && inputDevice != null)
            //{
            //    InputReportEventArgs args = new InputReportEventArgs(inputDevice, inputReport);
            //    args.RoutedEvent = InputManager.PreviewInputReportEvent;
                
            //    //if (inputManager != null)
            //    //    inputManager.ProcessInput(args);

            //    StagingAreaInputItem item = new StagingAreaInputItem(input, null);


            //    bool fCanceled = false;
            //    int devType = (int)inputDevice.DeviceType;

            //    // Pre-Process the input.
            //    if (InputDeviceEvents[devType]._preProcessInput != null)
            //    {
            //        PreProcessInputEventArgs preProcessInputEventArgs = new PreProcessInputEventArgs(item);
            //        InputDeviceEvents[devType]._preProcessInput(this, preProcessInputEventArgs);
            //        fCanceled = preProcessInputEventArgs._canceled;
            //    }

            //    if (!fCanceled)
            //    {
            //        // Pre-Notify the input.
            //        if (InputDeviceEvents[devType]._preNotifyInput != null)
            //            InputDeviceEvents[devType]._preNotifyInput(this, new NotifyInputEventArgs(item));

            //        // Raise the input event being processed.
            //        InputEventArgs input = item.Input;

            //        // Some input events are explicitly associated with
            //        // an element.  Those that are not instead are associated with
            //        // the target of the input device for this event.
            //        UIElement eventSource = input._source as UIElement;
            //        if (eventSource == null && input._inputDevice != null)
            //            eventSource = input._inputDevice.Target;
            //        if (eventSource != null)
            //            eventSource.RaiseEvent(input);

            //        // Post-Notify the input.
            //        if (InputDeviceEvents[devType]._postNotifyInput != null)
            //            InputDeviceEvents[devType]._postNotifyInput(this, new NotifyInputEventArgs(item));
            //        // Post-Process the input. This could modify the staging area.
            //        if (InputDeviceEvents[devType]._postProcessInput != null)
            //            InputDeviceEvents[devType]._postProcessInput(this, new ProcessInputEventArgs(item));

            //        switch (inputDevice.DeviceType)
            //        {
            //            case Microsoft.SPOT.Input.InputManager.InputDeviceType.Touch:
            //                TouchPostProcessInput(new ProcessInputEventArgs(item));
            //                break;
            //            case Microsoft.SPOT.Input.InputManager.InputDeviceType.Generic:
            //                GenericPostProcessInput(new ProcessInputEventArgs(item));
            //                break;
            //        }

            //        // PreviewInputReport --> InputReport
            //        if (item.Input._routedEvent == InputManager.PreviewInputReportEvent)
            //        {
            //            if (!item.Input.Handled)
            //            {
            //                InputReportEventArgs previewInputReport = (InputReportEventArgs)item.Input;
            //                InputReportEventArgs inputReport = new InputReportEventArgs(previewInputReport.Device, previewInputReport.Report);
            //                inputReport.RoutedEvent = InputManager.InputReportEvent;

            //                _currentStagingStack.Push(new StagingAreaInputItem(inputReport, item));
            //            }
            //        }

            //        if (input.Handled)
            //        {
            //            handled = true;
            //        }
            //    }
            //}

            return true;
        }

        private void TouchPostProcessInput(ProcessInputEventArgs e)
        {
            //if (!e.StagingItem.Input.Handled)
            //{
            //    RoutedEvent routedEvent = e.StagingItem.Input.RoutedEvent;
            //    if (routedEvent == InputManager.InputReportEvent)
            //    {
            //        InputReportEventArgs input = e.StagingItem.Input as InputReportEventArgs;
            //        if (input != null)
            //        {
            //            RawTouchInputReport report = input.Report as RawTouchInputReport;
            //            if (report != null)
            //            {
            //                TouchEventArgs args = new TouchEventArgs(this, report.Timestamp, report.Touches);
            //                UIElement target = report.Target;

            //                if (report.EventMessage == (byte)TouchMessages.Down)
            //                {
            //                    args.RoutedEvent = TouchEvents.TouchDownEvent;
            //                }
            //                else if (report.EventMessage == (byte)TouchMessages.Up)
            //                {
            //                    args.RoutedEvent = TouchEvents.TouchUpEvent;
            //                }
            //                else if (report.EventMessage == (byte)TouchMessages.Move)
            //                {
            //                    args.RoutedEvent = TouchEvents.TouchMoveEvent;
            //                }
            //                else
            //                    throw new Exception("Unknown touch event.");

            //                args.Source = (target == null ? _focus : target);
            //                e.PushInput(args, e.StagingItem);
            //            }
            //        }
            //    }
            //}
        }
        private void GenericPostProcessInput(ProcessInputEventArgs e)
        {
            //InputReportEventArgs input = e.StagingItem.Input as InputReportEventArgs;
            //if (input != null && input.RoutedEvent == InputManager.InputReportEvent)
            //{
            //    RawGenericInputReport report = input.Report as RawGenericInputReport;

            //    if (report != null)
            //    {
            //        if (!e.StagingItem.Input.Handled)
            //        {
            //            GenericEvent ge = (GenericEvent)report.InternalEvent;
            //            GenericEventArgs args = new GenericEventArgs(this, report.InternalEvent);

            //            args.RoutedEvent = GenericEvents.GenericStandardEvent;
            //            if (report.Target != null)
            //            {
            //                args.Source = report.Target;
            //            }

            //            e.PushInput(args, e.StagingItem);
            //        }
            //    }
            //}
        }
    }

    //public class DeviceEvents : DispatcherObject
    //{
    //    /// <summary>Subscribe for all input before it is processed</summary>
    //    public event PreProcessInputEventHandler PreProcessInput
    //    {
    //        add
    //        {
    //            VerifyAccess();

    //            // Add the handlers in reverse order so that handlers that
    //            // users add are invoked before handlers in the system.

    //            _preProcessInput = (PreProcessInputEventHandler)WeakDelegate.Combine(value, _preProcessInput);
    //        }
    //        remove
    //        {
    //            VerifyAccess();
    //            _preProcessInput = (PreProcessInputEventHandler)WeakDelegate.Remove(_preProcessInput, value);
    //        }
    //    }

    //    /// <summary>Subscribe for all input before it is notified</summary>
    //    public event NotifyInputEventHandler PreNotifyInput
    //    {
    //        add
    //        {
    //            VerifyAccess();

    //            // Add the handlers in reverse order so that handlers that
    //            // users add are invoked before handlers in the system.

    //            _preNotifyInput = (NotifyInputEventHandler)WeakDelegate.Combine(value, _preNotifyInput);
    //        }

    //        remove
    //        {
    //            VerifyAccess();
    //            _preNotifyInput = (NotifyInputEventHandler)WeakDelegate.Remove(_preNotifyInput, value);
    //        }

    //    }

    //    /// <summary>Subscribe to all input after it is notified</summary>
    //    public event NotifyInputEventHandler PostNotifyInput
    //    {
    //        add
    //        {
    //            VerifyAccess();

    //            // Add the handlers in reverse order so that handlers that
    //            // users add are invoked before handlers in the system.

    //            _postNotifyInput = (NotifyInputEventHandler)WeakDelegate.Combine(value, _postNotifyInput);
    //        }

    //        remove
    //        {
    //            VerifyAccess();

    //            _postNotifyInput = (NotifyInputEventHandler)WeakDelegate.Remove(_postNotifyInput, value);
    //        }
    //    }

    //    /// <summary>subscribe to all input after it is processed</summary>
    //    public event ProcessInputEventHandler PostProcessInput
    //    {
    //        add
    //        {
    //            VerifyAccess();

    //            // Add the handlers in reverse order so that handlers that
    //            // users add are invoked before handlers in the system.

    //            _postProcessInput = (ProcessInputEventHandler)WeakDelegate.Combine(value, _postProcessInput);
    //        }

    //        remove
    //        {
    //            VerifyAccess();

    //            _postProcessInput = (ProcessInputEventHandler)WeakDelegate.Remove(_postProcessInput, value);
    //        }
    //    }

    //    internal PreProcessInputEventHandler _preProcessInput;
    //    internal NotifyInputEventHandler _preNotifyInput;
    //    internal NotifyInputEventHandler _postNotifyInput;
    //    internal ProcessInputEventHandler _postProcessInput;

    //}
}
