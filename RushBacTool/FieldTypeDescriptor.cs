using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RushBacTool
{
    // For PropertyGrid
    public class FieldTypeDescriptorProvider(Type type) : TypeDescriptionProvider(TypeDescriptor.GetProvider(type))
    {
        public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type objectType, object instance)
        {
            var baseDescriptor = base.GetTypeDescriptor(objectType, instance);
            return new FieldTypeDescriptor(instance, baseDescriptor);
        }

        public static void RegisterTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                TypeDescriptor.AddProvider(new FieldTypeDescriptorProvider(type), type);
        }
    }

    public class FieldTypeDescriptor : CustomTypeDescriptor
    {
        public ICustomTypeDescriptor Parent { get; }
        public object Target { get; }

        public FieldDescriptor[] Fields { get; }
        public PropertyDescriptorCollection Properties { get; }
        public AttributeCollection Attributes { get; }

        public FieldTypeDescriptor(object target, ICustomTypeDescriptor parent) : base(parent)
        {
            Parent = parent;
            Target = target;
            if (Target != null)
            {
                FieldInfo[] fields = Target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                Properties = new PropertyDescriptorCollection(Parent.GetProperties().Cast<PropertyDescriptor>()
                    .Concat(fields.Select(f => (PropertyDescriptor)new FieldDescriptor(f)))
                    .ToArray());
            }
            else
                Properties = Parent.GetProperties();
            Attributes = new AttributeCollection(Parent.GetAttributes().Cast<Attribute>()
                .Append(new TypeConverterAttribute(typeof(ExpandableObjectConverter)))
                .Append(ReadOnlyAttribute.Yes)
                .ToArray());
        }

        public override AttributeCollection GetAttributes() => Attributes;
        public override PropertyDescriptorCollection GetProperties() => Properties;
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) => Properties;

        public override string GetClassName() => Parent.GetClassName();
        public override object GetPropertyOwner(PropertyDescriptor pd) => Target;
    }

    public class FieldDescriptor(FieldInfo fieldInfo) : PropertyDescriptor(fieldInfo.Name, GetAttributes(fieldInfo))
    {
        public FieldInfo FieldInfo { get; } = fieldInfo;

        public override Type ComponentType => FieldInfo.DeclaringType;
        public override Type PropertyType => FieldInfo.FieldType;
        public override bool IsReadOnly => FieldInfo.IsInitOnly;

        public override bool ShouldSerializeValue(object component) => false;
        public override bool CanResetValue(object component) => true;

        public override void ResetValue(object component)
        {
            Type type = PropertyType;
            if (type.IsValueType)
                SetValue(component, Activator.CreateInstance(type));
            else
                SetValue(component, null);
        }

        public override object GetValue(object component) => FieldInfo.GetValue(component);
        public override void SetValue(object component, object value) => FieldInfo.SetValue(component, value);

        static Attribute[] GetAttributes(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsAssignableTo(typeof(IList)))
                return [new TypeConverterAttribute(typeof(ListConverter))];
            return [];
        }
    }

    public class ListConverter : CollectionConverter
    {
        [return: NotNullIfNotNull(nameof(value))]
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value is not IList list)
                return PropertyDescriptorCollection.Empty;
            int count = list.Count;
            Type listType = value.GetType();
            PropertyDescriptor[] props = new PropertyDescriptor[count];
            for (int i = 0; i < count; i++)
                props[i] = new ListPropertyDescriptor(listType, list[i]?.GetType(), i);
            return new PropertyDescriptorCollection(props);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

        class ListPropertyDescriptor(Type listType, Type elementType, int index) : SimplePropertyDescriptor(listType, "[" + index + "]", elementType, null)
        {
            readonly int _index = index;

            public override object GetValue(object component)
            {
                if (component is IList list)
                    return list[_index];
                return null;
            }

            public override void SetValue(object component, object value)
            {
                if (component is IList list)
                {
                    list[_index] = value;
                    OnValueChanged(component, EventArgs.Empty);
                }
            }
        }
    }
}