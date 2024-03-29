﻿using NatCruise.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NatCruise.Wpf.Controls
{
    public class LogFieldSetupDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GradeDataTemplate { get; set; }

        public DataTemplate CommonDataTemplate { get; set; }

        public IEnumerable<string> GradeOptions { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var logFieldSetup = item as LogFieldSetup;
            if (logFieldSetup != null)
            {
                var field = logFieldSetup.Field;
                switch (field)
                {
                    case (nameof(Log.Grade)):
                    case (nameof(Log.ExportGrade)):
                    //{
                    //    var layoutFactory = new FrameworkElementFactory(typeof(LastChildWidthPanel));
                    //    var textBlockFactory = new FrameworkElementFactory(typeof(LastChildWidthPanel));
                    //    textBlockFactory.SetValue(TextBlock.TextProperty, logFieldSetup.Heading);
                    //    layoutFactory.AppendChild(textBlockFactory);

                    //    var editCtrlFactory = new FrameworkElementFactory(typeof(TextBox));
                    //    editCtrlFactory.SetBinding(ComboBox.TextProperty, new Binding(logFieldSetup.Field));
                    //    //editCtrlFactory.SetValue(ComboBox.ItemsSourceProperty, GradeOptions);

                    //    return new DataTemplate() { VisualTree = layoutFactory };
                    //}
                    case (nameof(Log.BoardFootRemoved)):
                    case (nameof(Log.BarkThickness)):
                    case (nameof(Log.CubicFootRemoved)):
                    case (nameof(Log.DIBClass)):
                    case (nameof(Log.GrossBoardFoot)):
                    case (nameof(Log.GrossCubicFoot)):
                    case (nameof(Log.LargeEndDiameter)):
                    case (nameof(Log.Length)):
                    case (nameof(Log.NetBoardFoot)):
                    case (nameof(Log.NetCubicFoot)):
                    case (nameof(Log.PercentRecoverable)):
                    case (nameof(Log.SeenDefect)):
                    case (nameof(Log.SmallEndDiameter)):
                        {
                            // since we are returning a DataTemplate we have to build the ui using
                            // a FrameworkElementFactory

                            // create layout panel for our field
                            var layoutFactory = new FrameworkElementFactory(typeof(LastChildWidthPanel));
                            layoutFactory.SetValue(LastChildWidthPanel.LastChildWidthProperty, 60.0);
                            var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                            textBlockFactory.SetValue(TextBlock.TextProperty, logFieldSetup.Heading);
                            layoutFactory.AppendChild(textBlockFactory);

                            // create edit control
                            var editCtrlFactory = new FrameworkElementFactory(typeof(TextBox));
                            editCtrlFactory.SetBinding(TextBox.TextProperty,
                                    new Binding("DataContext.Log." + logFieldSetup.Field)
                                    {
                                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                                        { AncestorType = typeof(ItemsControl) },
                                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                                        Delay = 500,
                                    }
                                );

                            if (field != nameof(Log.Grade) || field != nameof(Log.ExportGrade))
                            {
                                var regexMask = (string)App.Current.Resources["const_regex_real"];
                                editCtrlFactory.SetValue(NatCruise.Wpf.Controls.TextBoxHelper.RegexMaskProperty, regexMask);
                            }
                            layoutFactory.AppendChild(editCtrlFactory);

                            return new DataTemplate() { VisualTree = layoutFactory };
                        }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}