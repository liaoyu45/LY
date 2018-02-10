using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace PadKeyboard {
    [MarkupExtensionReturnType(typeof(object))]
    public class LazyBindingExtension : MarkupExtension {
        public LazyBindingExtension() { }

        public LazyBindingExtension(string path) {
            Path = new PropertyPath(path);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            var service = serviceProvider.GetService
                (typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null)
                return null;

            mTarget = service.TargetObject as FrameworkElement;
            mProperty = service.TargetProperty as DependencyProperty;
            if (mTarget != null && mProperty != null) {
                // 侦听IsVisible属性的更改，以在界面元素显示时通过OnIsVisibleChanged
                // 函数添加绑定
                mTarget.IsVisibleChanged += OnIsVisibleChanged;
                return null;
            } else {
                return CreateBinding().ProvideValue(serviceProvider);
            }
        }

        private void OnIsVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e) {
            // 添加绑定
            BindingOperations.SetBinding(mTarget, mProperty, CreateBinding());
        }

        private Binding CreateBinding() // 创建绑定类型实例
        {
            Binding binding = new Binding(Path.Path);
            if (Source != null)
                binding.Source = Source;
            if (RelativeSource != null)
                binding.RelativeSource = RelativeSource;
            if (ElementName != null)
                binding.ElementName = ElementName;
            binding.Converter = Converter;
            binding.ConverterParameter = ConverterParameter;
            return binding;
        }

        #region Fields
        private FrameworkElement mTarget = null;
        private DependencyProperty mProperty = null;
        #endregion

        #region Properties
        public object Source { get; set; }
        public RelativeSource RelativeSource { get; set; }
        public string ElementName { get; set; }
        public PropertyPath Path { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        #endregion
    }
}
