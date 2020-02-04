using UnityEngine;
using System.Collections;
using System;
using Oculus.Avatar;

public class OvrAvatarSkinnedMeshPBSV2RenderComponent : OvrAvatarRenderComponent
{
    // DIY COLLIDERS ON HANDS
    private struct FingerBone {
        public readonly float Radius;
        public readonly float Height;
        public FingerBone(float radius, float height) {
            Radius = radius;
            Height = height;
        }

        public Vector3 GetCenter(bool isLeftHand) {
            return new Vector3(((isLeftHand) ? -1 : 1) * Height / 2.0f, 0, 0);
        }
    };

    // DIY COLLIDERS ON HANDS
    private readonly FingerBone Phalanges = new FingerBone(0.01f, 0.03f);
    // DIY COLLIDERS ON HANDS
    private readonly FingerBone Metacarpals = new FingerBone(0.01f, 0.05f);

    public OvrAvatarMaterialManager AvatarMaterialManager;
    bool PreviouslyActive = false;
    bool IsCombinedMaterial = false;
    ovrAvatarExpressiveParameters ExpressiveParameters;
    bool EnableExpressive = false;

    private const string MAIN_MATERIAL_NAME = "main_material";
    private const string EYE_MATERIAL_NAME = "eye_material";
    private const string DEFAULT_MATERIAL_NAME = "_material";

    internal void Initialize(
        IntPtr renderPart,
        ovrAvatarRenderPart_SkinnedMeshRenderPBS_V2 skinnedMeshRender,
        OvrAvatarMaterialManager materialManager,
        int thirdPersonLayer, 
        int firstPersonLayer, 
        int sortOrder,
        bool isCombinedMaterial,
        ovrAvatarAssetLevelOfDetail lod,
        bool assignExpressiveParams,
        OvrAvatar avatar,
        bool isControllerModel)
    {
        AvatarMaterialManager = materialManager;
        IsCombinedMaterial = isCombinedMaterial;

        mesh = CreateSkinnedMesh(
            skinnedMeshRender.meshAssetID, 
            skinnedMeshRender.visibilityMask, 
            thirdPersonLayer,
            firstPersonLayer, 
            sortOrder);

        EnableExpressive = assignExpressiveParams;

#if UNITY_ANDROID
        var singleComponentShader = EnableExpressive 
            ? avatar.Skinshaded_Expressive_VertFrag_SingleComponent 
            : avatar.Skinshaded_VertFrag_SingleComponent;
#else
        var singleComponentShader = EnableExpressive
            ? avatar.Skinshaded_Expressive_SurfaceShader_SingleComponent
            : avatar.Skinshaded_SurfaceShader_SingleComponent;
#endif
        var combinedComponentShader = EnableExpressive
            ? avatar.Skinshaded_Expressive_VertFrag_CombinedMesh
            : avatar.Skinshaded_VertFrag_CombinedMesh;

        var mainShader = IsCombinedMaterial ? combinedComponentShader : singleComponentShader;

        if (isControllerModel)
        {
            mainShader = Shader.Find("OvrAvatar/AvatarPBRV2Simple");
        }

       AvatarLogger.Log("OvrAvatarSkinnedMeshPBSV2RenderComponent Shader is: " + mainShader != null 
           ? mainShader.name : "null");

        if (EnableExpressive)
        {
            ExpressiveParameters = CAPI.ovrAvatar_GetExpressiveParameters(avatar.sdkAvatar);

            var eyeShader = avatar.EyeLens;

            Material[] matArray = new Material[2];
            matArray[0] = CreateAvatarMaterial(gameObject.name + MAIN_MATERIAL_NAME, mainShader);
            matArray[1] = CreateAvatarMaterial(gameObject.name + EYE_MATERIAL_NAME, eyeShader);

            if (avatar.UseTransparentRenderQueue)
            {
                // Initialize shader to use transparent render queue with alpha blending
                matArray[0].SetOverrideTag("RenderType", "Transparent");
                matArray[0].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                matArray[0].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                matArray[0].EnableKeyword("_ALPHATEST_ON");
                matArray[0].EnableKeyword("_ALPHABLEND_ON");
                matArray[0].EnableKeyword("_ALPHAPREMULTIPLY_ON");
                matArray[0].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                // Initialize shader to use geometry render queue with no blending
                matArray[0].SetOverrideTag("RenderType", "Opaque");
                matArray[0].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                matArray[0].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                matArray[0].DisableKeyword("_ALPHATEST_ON");
                matArray[0].DisableKeyword("_ALPHABLEND_ON");
                matArray[0].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                matArray[0].renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }
            // Eye lens shader queue is transparent and set from shader
            matArray[1].renderQueue = -1;
            mesh.materials = matArray;
        }
        else
        {
            mesh.sharedMaterial = CreateAvatarMaterial(gameObject.name + DEFAULT_MATERIAL_NAME, mainShader);
            if (avatar.UseTransparentRenderQueue && !isControllerModel)
            {
                // Initialize shader to use transparent render queue with alpha blending
                mesh.sharedMaterial.SetOverrideTag("RenderType", "Transparent");
                mesh.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mesh.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mesh.sharedMaterial.EnableKeyword("_ALPHATEST_ON");
                mesh.sharedMaterial.EnableKeyword("_ALPHABLEND_ON");
                mesh.sharedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mesh.sharedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                // Initialize shader to use geometry render queue with no blending
                mesh.sharedMaterial.SetOverrideTag("RenderType", "Opaque");
                mesh.sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mesh.sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mesh.sharedMaterial.DisableKeyword("_ALPHATEST_ON");
                mesh.sharedMaterial.DisableKeyword("_ALPHABLEND_ON");
                mesh.sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mesh.sharedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }
        }
        bones = mesh.bones;

        if (IsCombinedMaterial)
        {
            AvatarMaterialManager.SetRenderer(mesh);
            InitializeCombinedMaterial(renderPart, (int)lod - 1);
            AvatarMaterialManager.OnCombinedMeshReady();
        }

        // DIY COLLIDERS ON HANDS
        foreach(Transform bone in bones) {
            if (!bone.name.Contains("ignore")) {
                CreateCollider(bone);
            }
        }
        Rigidbody handRb = gameObject.AddComponent<Rigidbody>();
        handRb.isKinematic = true;
        handRb.useGravity = false;
        handRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // DIY COLLIDERS ON HANDS
    private void CreateCollider(Transform transform) {
        if (!transform.gameObject.GetComponent(typeof(CapsuleCollider)) && !transform.gameObject.GetComponent(typeof(SphereCollider)) && transform.name.Contains("hands")) {
            if (transform.name.Contains("thumb") || transform.name.Contains("index") || transform.name.Contains("middle") || transform.name.Contains("ring") || transform.name.Contains("pinky")) {
                if (!transform.name.EndsWith("0")) {
                    CapsuleCollider collider = transform.gameObject.AddComponent<CapsuleCollider>();
                    if (!transform.name.EndsWith("1")) {
                        collider.radius = Phalanges.Radius;
                        collider.height = Phalanges.Height;
                        collider.center = Phalanges.GetCenter(transform.name.Contains("_l_"));
                        collider.direction = 0;
                    } else {
                        collider.radius = Metacarpals.Radius;
                        collider.height = Metacarpals.Height;
                        collider.center = Metacarpals.GetCenter(transform.name.Contains("_l_"));
                        collider.direction = 0;
                    }
                }
            } else if (transform.name.Contains("grip")) {
                SphereCollider collider = transform.gameObject.AddComponent<SphereCollider>();
                collider.radius = 0.04f;
                // collider.size = new Vector3(0.08f, 0.08f, 0.02f);
                collider.center = new Vector3( ((transform.name.Contains("_l_")) ? -1 : 1) * 0.01f, 0.01f, 0.02f);

                // GameObject subParent = new GameObject();
                // subParent.transform.parent = transform;
                // subParent.transform.position = new Vector3(0.0224f, 0.0165f, 0.0597f);
                // subParent.transform.rotation = new Quaternion(195.734f, 63.07899f, -50.599f, 0f);
                // // subParent.transform.Rotate(0.702f, 5.113f, -98.21201f, 0f);
                // BoxCollider collider = subParent.AddComponent<BoxCollider>();
                // collider.center = new Vector3(((transform.name.Contains("_l_")) ? -1 : 1) * 0.01f, 0.02f, 0.024f);
                // collider.size = new Vector3(0.08f, 0.12f, 0.02f);
            }
        }
    }

    public void UpdateSkinnedMeshRender(
        OvrAvatarComponent component, 
        OvrAvatar avatar, 
        IntPtr renderPart)
    {
        ovrAvatarVisibilityFlags visibilityMask 
            = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetVisibilityMask(renderPart);

        ovrAvatarTransform localTransform 
            = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetTransform(renderPart);

        UpdateSkinnedMesh(avatar, bones, localTransform, visibilityMask, renderPart);

        bool isActive = gameObject.activeSelf;

        if (mesh != null && !PreviouslyActive && isActive)
        {
            if (!IsCombinedMaterial)
            {
                InitializeSingleComponentMaterial(renderPart, (int)avatar.LevelOfDetail - 1);
            }
        }

        PreviouslyActive = isActive;
    }

    private void InitializeSingleComponentMaterial(IntPtr renderPart, int lodIndex)
    {
        ovrAvatarPBSMaterialState materialState =
            CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetPBSMaterialState(renderPart);

        int componentType = (int)OvrAvatarMaterialManager.GetComponentType(gameObject.name);

        var defaultProperties = AvatarMaterialManager.DefaultAvatarConfig.ComponentMaterialProperties;

        var diffuseTexture = OvrAvatarComponent.GetLoadedTexture(materialState.albedoTextureID);
        var normalTexture = OvrAvatarComponent.GetLoadedTexture(materialState.normalTextureID);
        var metallicTexture = OvrAvatarComponent.GetLoadedTexture(materialState.metallicnessTextureID);

        if (diffuseTexture == null)
        {
            diffuseTexture = AvatarMaterialManager.DiffuseFallbacks[lodIndex];
        }
            
        if (normalTexture == null)
        {
            normalTexture = AvatarMaterialManager.NormalFallbacks[lodIndex];
        }

        if (metallicTexture == null)
        {
            metallicTexture = AvatarMaterialManager.DiffuseFallbacks[lodIndex];
        }

        mesh.materials[0].SetTexture(OvrAvatarMaterialManager.AVATAR_SHADER_MAINTEX, diffuseTexture);
        mesh.materials[0].SetTexture(OvrAvatarMaterialManager.AVATAR_SHADER_NORMALMAP, normalTexture);
        mesh.materials[0].SetTexture(OvrAvatarMaterialManager.AVATAR_SHADER_ROUGHNESSMAP, metallicTexture);

        mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_COLOR, 
            materialState.albedoMultiplier);

        mesh.materials[0].SetFloat(OvrAvatarMaterialManager.AVATAR_SHADER_DIFFUSEINTENSITY,
            defaultProperties[componentType].DiffuseIntensity);

        mesh.materials[0].SetFloat(OvrAvatarMaterialManager.AVATAR_SHADER_RIMINTENSITY,
            defaultProperties[componentType].RimIntensity);

        mesh.materials[0].SetFloat(OvrAvatarMaterialManager.AVATAR_SHADER_REFLECTIONINTENSITY,
            defaultProperties[componentType].ReflectionIntensity);

        mesh.GetClosestReflectionProbes(AvatarMaterialManager.ReflectionProbes);
        if (AvatarMaterialManager.ReflectionProbes != null &&
            AvatarMaterialManager.ReflectionProbes.Count > 0)
        {
            mesh.materials[0].SetTexture(OvrAvatarMaterialManager.AVATAR_SHADER_CUBEMAP,
                AvatarMaterialManager.ReflectionProbes[0].probe.texture);
        }

        if (EnableExpressive)
        {
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_IRIS_COLOR, 
                ExpressiveParameters.irisColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_LIP_COLOR,
                ExpressiveParameters.lipColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_BROW_COLOR,
                ExpressiveParameters.browColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_LASH_COLOR,
                ExpressiveParameters.lashColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_SCLERA_COLOR,
                ExpressiveParameters.scleraColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_GUM_COLOR,
                ExpressiveParameters.gumColor);
            mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_TEETH_COLOR,
                ExpressiveParameters.teethColor);
            mesh.materials[0].SetFloat(OvrAvatarMaterialManager.AVATAR_SHADER_LIP_SMOOTHNESS,
                ExpressiveParameters.lipSmoothness);
        }
    }

    private void InitializeCombinedMaterial(IntPtr renderPart, int lodIndex)
    {
        ovrAvatarPBSMaterialState[] materialStates = CAPI.ovrAvatar_GetBodyPBSMaterialStates(renderPart);

        if (materialStates.Length == (int)ovrAvatarBodyPartType.Count)
        {
            AvatarMaterialManager.CreateTextureArrays();

            AvatarMaterialManager.LocalAvatarConfig = AvatarMaterialManager.DefaultAvatarConfig;
            var localProperties = AvatarMaterialManager.LocalAvatarConfig.ComponentMaterialProperties;

            AvatarLogger.Log("InitializeCombinedMaterial - Loading Material States");

            for (int i = 0; i < materialStates.Length; i++)
            {
                localProperties[i].TypeIndex = (ovrAvatarBodyPartType)i;
                localProperties[i].Color = materialStates[i].albedoMultiplier;

                var diffuse = OvrAvatarComponent.GetLoadedTexture(materialStates[i].albedoTextureID);
                var normal = OvrAvatarComponent.GetLoadedTexture(materialStates[i].normalTextureID);
                var roughness = OvrAvatarComponent.GetLoadedTexture(materialStates[i].metallicnessTextureID);

                localProperties[i].Textures[(int)OvrAvatarMaterialManager.TextureType.DiffuseTextures]
                    = diffuse == null ? AvatarMaterialManager.DiffuseFallbacks[lodIndex] : diffuse;

                localProperties[i].Textures[(int)OvrAvatarMaterialManager.TextureType.NormalMaps]
                    = normal == null ? AvatarMaterialManager.NormalFallbacks[lodIndex] : normal;

                localProperties[i].Textures[(int)OvrAvatarMaterialManager.TextureType.RoughnessMaps]
                    = roughness == null ? AvatarMaterialManager.DiffuseFallbacks[lodIndex] : roughness;

                AvatarLogger.Log(localProperties[i].TypeIndex.ToString());
                AvatarLogger.Log(AvatarLogger.Tab + "Diffuse: " + materialStates[i].albedoTextureID);
                AvatarLogger.Log(AvatarLogger.Tab + "Normal: " + materialStates[i].normalTextureID);
                AvatarLogger.Log(AvatarLogger.Tab + "Metallic: " + materialStates[i].metallicnessTextureID);
            }

            if (EnableExpressive)
            {
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_IRIS_COLOR,
                    ExpressiveParameters.irisColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_LIP_COLOR,
                    ExpressiveParameters.lipColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_BROW_COLOR,
                    ExpressiveParameters.browColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_LASH_COLOR,
                    ExpressiveParameters.lashColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_SCLERA_COLOR,
                    ExpressiveParameters.scleraColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_GUM_COLOR,
                    ExpressiveParameters.gumColor);
                mesh.materials[0].SetVector(OvrAvatarMaterialManager.AVATAR_SHADER_TEETH_COLOR,
                    ExpressiveParameters.teethColor);
                mesh.materials[0].SetFloat(OvrAvatarMaterialManager.AVATAR_SHADER_LIP_SMOOTHNESS,
                    ExpressiveParameters.lipSmoothness);
            }

            AvatarMaterialManager.ValidateTextures();
        }   
    }
}
