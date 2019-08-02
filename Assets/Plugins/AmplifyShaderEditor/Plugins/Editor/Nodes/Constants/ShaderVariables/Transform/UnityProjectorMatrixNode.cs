// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
    [System.Serializable]
    [NodeAttributes("Projector Matrix", "Matrix Transform", "Current Projector Clip matrix. To be used when working with Unity projector.")]
    public sealed class UnityProjectorMatrixNode : ConstantShaderVariable
    {
        protected override void CommonInit(int uniqueId)
        {
            base.CommonInit(uniqueId);
            ChangeOutputProperties(0, "Out", WirePortDataType.FLOAT4x4);
            m_value = "unity_Projector";
        }

        public override string GenerateShaderForOutput(int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar)
        {
            dataCollector.AddToUniforms(UniqueId, "float4x4 unity_Projector;");
            return base.GenerateShaderForOutput(outputId, ref dataCollector, ignoreLocalvar);
        }
    }
}
