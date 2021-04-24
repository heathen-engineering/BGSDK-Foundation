using HeathenEngineering.BGSDK.Engine;
using System;
using UnityEngine;

namespace HeathenEngineering.BGSDK.Examples
{
    /// <summary>
    /// Example token properties
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a simple example of how to create custom properties for your token definitions.
    /// The intent is that you create your own custom datamodel and wrapp it in a custom ScriptableObject such as this.
    /// Once done you can create these properties items in your asset folder as you would any other ScriptableObject.
    /// You can drag and drop this properties object to your Token Definition where it will be used to deserialize custom properts data.
    /// </para>
    /// <para>
    /// To create your own custom properties object you only need to defein a structure or class which is serializable and contains the nessisary fields for your desired properties model.
    /// <see cref="ExampleTokenPropertiesDataModel"/> is what is used in this particular example. You can review its source code and see that it is a simple struct with a number of fields and is marked as serializable.
    /// The final step is to create a new class object which inherits from <see cref="TokenProperties{T}"/> where T should be the struct or class you defined such as <see cref="ExampleTokenPropertiesDataModel"/> as in this example.
    /// Be sure to decorate the class with 
    /// <code>
    /// [CreateAssetMenu(menuName = "Custom/Properties/[NameOfProperty]")]
    /// </code>
    /// This will allow you to create the property object in your asset database via the Create menu in Unity Editor.
    /// </para>
    /// <para>
    /// Note that you do not need to create any logic or fields in the ScriptableObject class, all of the required fields are part of the <see cref="TokenProperties{T}"/> object.
    /// This is very much like creating custom UnityEvent{T} objects.
    /// </para>
    /// </remarks>
    [CreateAssetMenu(menuName = "Blockchain Game SDK/Examples/Token Properties")]
    public class ExampleTokenProperties : TokenProperties<ExampleTokenPropertiesDataModel>
    { }

    /// <summary>
    /// A example data model used in the <see cref="ExampleTokenProperties"/> object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is simply an example and not intended for production use.
    /// This struct demonstrates a complex data model suitable for use in token properties
    /// </para>
    /// </remarks>
    [Serializable]
    public struct ExampleTokenPropertiesDataModel
    {
        [Serializable]
        public struct CompositData
        {
            public string justAnExample;
            public int someNumber;
        }

        public string someString;
        public int someInt;
        public double someDouble;
        public CompositData[] dataArrayExample;
    }
}
