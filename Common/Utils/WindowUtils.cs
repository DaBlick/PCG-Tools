// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Cursor = System.Windows.Input.Cursor;
using MessageBox = System.Windows.MessageBox;

namespace Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WindowUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public enum EMessageBoxButton
        {
            Ok,
            YesNo,
            YesNoCancel
        } ;


        /// <summary>
        /// 
        /// </summary>
        public enum EMessageBoxImage
        {
            Error,
            Warning,
            Exclamation,
            Information
        } ;


        /// <summary>
        /// 
        /// </summary>
        public enum EMessageBoxResult
        {
            Ok,
            Yes,
            No,
            None,
            Cancel
        } ;


        /// <summary>
        /// 
        /// </summary>
        public enum ECursor
        {
            Wait,
            Arrow
        } ;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <param name="messageBoxButton"></param>
        /// <param name="messageBoxImage"></param>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        public static EMessageBoxResult ShowMessageBox(Window window, string text, [NotNull] string title,
            EMessageBoxButton messageBoxButton,
            EMessageBoxImage messageBoxImage, EMessageBoxResult messageBoxResult)
        {
            Debug.Assert(!string.IsNullOrEmpty(text));
            Debug.Assert(!string.IsNullOrEmpty(title));
            
            MessageBoxButton button;
            switch (messageBoxButton)
            {
                case EMessageBoxButton.Ok:
                    button = MessageBoxButton.OK;
                    break;

                case EMessageBoxButton.YesNo:
                    button = MessageBoxButton.YesNo;
                    break;
                
                case EMessageBoxButton.YesNoCancel:
                    button = MessageBoxButton.YesNoCancel;
                    break;

                default:
                    throw new ApplicationException("Illegal message box button");
            }

            MessageBoxImage image;
            switch (messageBoxImage)
            {
            case EMessageBoxImage.Error:
                image = MessageBoxImage.Error;
                break;

            case EMessageBoxImage.Warning:
                image = MessageBoxImage.Warning;
                break;

            case EMessageBoxImage.Exclamation:
                image = MessageBoxImage.Exclamation;
                break;

            case EMessageBoxImage.Information:
                image = MessageBoxImage.Information;
                break;

            default:
                throw new ApplicationException("Illegal message box image");
            }

            MessageBoxResult result;
            switch (messageBoxResult)
            {
            case EMessageBoxResult.Ok:
                result = MessageBoxResult.OK;
                break;

            case EMessageBoxResult.Yes:
                result = MessageBoxResult.Yes;
                break;

            case EMessageBoxResult.No:
                result = MessageBoxResult.No;
                break;

            case EMessageBoxResult.None:
                result = MessageBoxResult.None;
                break;

            case EMessageBoxResult.Cancel:
                result = MessageBoxResult.Cancel;
                break;

            default:
                throw new ApplicationException("Illegal message box result");
            }

            MessageBoxResult dialogResult = window == null ? MessageBox.Show(text, title, button, image, result) : 
                                                MessageBox.Show(window, text, title, button, image, result);

            EMessageBoxResult returnResult;
            switch (dialogResult)
            {
            case MessageBoxResult.OK:
                returnResult = EMessageBoxResult.Ok;
                break;

            case MessageBoxResult.Yes:
                returnResult = EMessageBoxResult.Yes;
                break;

            case MessageBoxResult.No:
                returnResult = EMessageBoxResult.No;
                break;

            case MessageBoxResult.Cancel:
                returnResult = EMessageBoxResult.Cancel;
                break;

            case MessageBoxResult.None:
                returnResult = EMessageBoxResult.None;
                break;

            default:
                throw new ApplicationException("Illegal message box result");
            }

            return returnResult;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eCursor"></param>
        public static void SetCursor(ECursor eCursor)
        {
            Cursor cursor;
            switch (eCursor)
            {
                case ECursor.Wait:
                cursor = Cursors.Wait;
                    break;

                case ECursor.Arrow:
                    cursor = Cursors.Arrow;
                    break;

                default:
                    throw new ApplicationException("Illegal cursor");
            }

            Mouse.OverrideCursor = cursor;
        }
    }
}