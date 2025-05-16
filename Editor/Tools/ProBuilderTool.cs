using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json.Linq;
using TheBifrost.Unity;
using TheBifrost.Utils;

#if PROBUILDER_PACKAGE
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEditor.ProBuilder;
#endif

namespace TheBifrost.Tools
{
    /// <summary>
    /// Tool for creating and manipulating ProBuilder meshes in Unity
    /// </summary>
    public class ProBuilderTool : McpToolBase
    {
        public ProBuilderTool()
        {
            Name = "probuilder";
            Description = "Creates and manipulates ProBuilder meshes";
            IsAsync = false;
        }

        /// <summary>
        /// Execute the ProBuilder tool with the provided parameters
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            // Extract the operation type
            string operation = parameters["operation"]?.ToObject<string>();
            
            if (string.IsNullOrEmpty(operation))
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    "Required parameter 'operation' not provided", 
                    "validation_error"
                );
            }

#if PROBUILDER_PACKAGE
            // Execute the appropriate operation
            switch (operation.ToLower())
            {
                case "create_primitive":
                    return CreatePrimitive(parameters);
                case "extrude_faces":
                    return ExtrudeFaces(parameters);
                case "bevel_edges":
                    return BevelEdges(parameters);
                case "set_material":
                    return SetMaterial(parameters);
                case "merge_objects":
                    return MergeObjects(parameters);
                case "boolean_operation":
                    return BooleanOperation(parameters);
                case "subdivide_faces":
                    return SubdivideFaces(parameters);
                case "triangulate_faces":
                    return TriangulateFaces(parameters);
                case "check_installed":
                    return new JObject
                    {
                        ["success"] = true,
                        ["installed"] = true,
                        ["message"] = "ProBuilder is installed"
                    };
                default:
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"Unknown operation: {operation}", 
                        "unknown_operation"
                    );
            }
#else
            // ProBuilder is not installed
            if (operation.ToLower() == "check_installed")
            {
                return new JObject
                {
                    ["success"] = true,
                    ["installed"] = false,
                    ["message"] = "ProBuilder is not installed"
                };
            }
            
            return TheBifrostSocketHandler.CreateErrorResponse(
                "ProBuilder package is not installed. Please install it via Package Manager.", 
                "package_not_installed"
            );
#endif
        }

#if PROBUILDER_PACKAGE
        /// <summary>
        /// Create a ProBuilder primitive
        /// </summary>
        private JObject CreatePrimitive(JObject parameters)
        {
            try
            {
                // Extract parameters
                string shapeType = parameters["shapeType"]?.ToObject<string>() ?? "Cube";
                Vector3 position = ExtractVector3(parameters["position"], Vector3.zero);
                Vector3 size = ExtractVector3(parameters["size"], Vector3.one);
                string name = parameters["name"]?.ToObject<string>() ?? shapeType;
                
                // Create the shape
                ProBuilderMesh mesh = null;
                
                switch (shapeType.ToLower())
                {
                    case "cube":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Cube, size);
                        break;
                    case "sphere":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Sphere, size);
                        break;
                    case "cylinder":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Cylinder, size);
                        break;
                    case "plane":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Plane, size);
                        break;
                    case "stairs":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Stairs, size);
                        break;
                    case "door":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Door, size);
                        break;
                    case "pipe":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Pipe, size);
                        break;
                    case "cone":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Cone, size);
                        break;
                    case "prism":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Prism, size);
                        break;
                    case "torus":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Torus, size);
                        break;
                    case "arch":
                        mesh = ShapeGenerator.CreateShape(ShapeType.Arch, size);
                        break;
                    default:
                        return TheBifrostSocketHandler.CreateErrorResponse(
                            $"Unknown shape type: {shapeType}", 
                            "unknown_shape_type"
                        );
                }
                
                // Set the position and name
                mesh.gameObject.transform.position = position;
                mesh.gameObject.name = name;
                
                // Rebuild the mesh
                mesh.ToMesh();
                mesh.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully created {shapeType} at position {position}",
                    ["instanceId"] = mesh.gameObject.GetInstanceID()
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error creating primitive: {ex.Message}", 
                    "creation_error"
                );
            }
        }
        
        /// <summary>
        /// Extrude faces on a ProBuilder mesh
        /// </summary>
        private JObject ExtrudeFaces(JObject parameters)
        {
            try
            {
                // Extract parameters
                int instanceId = parameters["instanceId"]?.ToObject<int>() ?? 0;
                float extrudeDistance = parameters["distance"]?.ToObject<float>() ?? 1.0f;
                bool asGroup = parameters["asGroup"]?.ToObject<bool>() ?? true;
                
                // Find the GameObject
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (gameObject == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} not found", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder mesh
                ProBuilderMesh mesh = gameObject.GetComponent<ProBuilderMesh>();
                if (mesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} is not a ProBuilder mesh", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Get all faces
                IList<Face> faces = mesh.faces;
                
                // Extrude the faces
                mesh.Extrude(faces, ExtrudeMethod.FaceNormal, extrudeDistance);
                
                // Rebuild the mesh
                mesh.ToMesh();
                mesh.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully extruded faces on {gameObject.name}"
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error extruding faces: {ex.Message}", 
                    "extrude_error"
                );
            }
        }
        
        /// <summary>
        /// Bevel edges on a ProBuilder mesh
        /// </summary>
        private JObject BevelEdges(JObject parameters)
        {
            try
            {
                // Extract parameters
                int instanceId = parameters["instanceId"]?.ToObject<int>() ?? 0;
                float bevelDistance = parameters["distance"]?.ToObject<float>() ?? 0.1f;
                
                // Find the GameObject
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (gameObject == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} not found", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder mesh
                ProBuilderMesh mesh = gameObject.GetComponent<ProBuilderMesh>();
                if (mesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} is not a ProBuilder mesh", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Get all edges
                IList<Edge> edges = new List<Edge>();
                foreach (Face face in mesh.faces)
                {
                    edges.AddRange(face.edges);
                }
                
                // Bevel the edges
                mesh.Bevel(edges, bevelDistance);
                
                // Rebuild the mesh
                mesh.ToMesh();
                mesh.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully beveled edges on {gameObject.name}"
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error beveling edges: {ex.Message}", 
                    "bevel_error"
                );
            }
        }
        
        /// <summary>
        /// Set material on a ProBuilder mesh
        /// </summary>
        private JObject SetMaterial(JObject parameters)
        {
            try
            {
                // Extract parameters
                int instanceId = parameters["instanceId"]?.ToObject<int>() ?? 0;
                string materialPath = parameters["materialPath"]?.ToObject<string>();
                
                // Find the GameObject
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (gameObject == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} not found", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder mesh
                ProBuilderMesh mesh = gameObject.GetComponent<ProBuilderMesh>();
                if (mesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} is not a ProBuilder mesh", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Load the material
                Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                if (material == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"Material at path '{materialPath}' not found", 
                        "material_not_found"
                    );
                }
                
                // Set the material
                MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                }
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully set material on {gameObject.name}"
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error setting material: {ex.Message}", 
                    "material_error"
                );
            }
        }
        
        /// <summary>
        /// Merge multiple ProBuilder meshes
        /// </summary>
        private JObject MergeObjects(JObject parameters)
        {
            try
            {
                // Extract parameters
                JArray instanceIds = parameters["instanceIds"] as JArray;
                string resultName = parameters["resultName"]?.ToObject<string>() ?? "MergedMesh";
                
                if (instanceIds == null || instanceIds.Count < 2)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        "At least two instance IDs must be provided", 
                        "validation_error"
                    );
                }
                
                // Find the GameObjects
                List<ProBuilderMesh> meshes = new List<ProBuilderMesh>();
                foreach (JToken token in instanceIds)
                {
                    int id = token.ToObject<int>();
                    GameObject obj = EditorUtility.InstanceIDToObject(id) as GameObject;
                    if (obj != null)
                    {
                        ProBuilderMesh mesh = obj.GetComponent<ProBuilderMesh>();
                        if (mesh != null)
                        {
                            meshes.Add(mesh);
                        }
                    }
                }
                
                if (meshes.Count < 2)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        "At least two valid ProBuilder meshes must be provided", 
                        "validation_error"
                    );
                }
                
                // Merge the meshes
                ProBuilderMesh result = ProBuilderMesh.Create();
                result.gameObject.name = resultName;
                CombineMeshes.Combine(meshes, result);
                
                // Rebuild the mesh
                result.ToMesh();
                result.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully merged {meshes.Count} meshes into {resultName}",
                    ["instanceId"] = result.gameObject.GetInstanceID()
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error merging objects: {ex.Message}", 
                    "merge_error"
                );
            }
        }
        
        /// <summary>
        /// Perform a boolean operation on two ProBuilder meshes
        /// </summary>
        private JObject BooleanOperation(JObject parameters)
        {
            try
            {
                // Extract parameters
                int firstId = parameters["firstId"]?.ToObject<int>() ?? 0;
                int secondId = parameters["secondId"]?.ToObject<int>() ?? 0;
                string operation = parameters["booleanType"]?.ToObject<string>() ?? "union";
                string resultName = parameters["resultName"]?.ToObject<string>() ?? "BooleanResult";
                
                // Find the GameObjects
                GameObject firstObj = EditorUtility.InstanceIDToObject(firstId) as GameObject;
                GameObject secondObj = EditorUtility.InstanceIDToObject(secondId) as GameObject;
                
                if (firstObj == null || secondObj == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        "Both GameObjects must exist", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder meshes
                ProBuilderMesh firstMesh = firstObj.GetComponent<ProBuilderMesh>();
                ProBuilderMesh secondMesh = secondObj.GetComponent<ProBuilderMesh>();
                
                if (firstMesh == null || secondMesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        "Both GameObjects must have ProBuilder meshes", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Determine the boolean operation
                BooleanOp boolOp;
                switch (operation.ToLower())
                {
                    case "union":
                        boolOp = BooleanOp.Union;
                        break;
                    case "subtraction":
                    case "subtract":
                        boolOp = BooleanOp.Subtraction;
                        break;
                    case "intersection":
                    case "intersect":
                        boolOp = BooleanOp.Intersection;
                        break;
                    default:
                        return TheBifrostSocketHandler.CreateErrorResponse(
                            $"Unknown boolean operation: {operation}", 
                            "unknown_operation"
                        );
                }
                
                // Perform the boolean operation
                ProBuilderMesh result = ProBuilderMesh.Create();
                result.gameObject.name = resultName;
                Boolean.MeshImplicitOperation(firstMesh, secondMesh, result, boolOp);
                
                // Rebuild the mesh
                result.ToMesh();
                result.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully performed {operation} operation",
                    ["instanceId"] = result.gameObject.GetInstanceID()
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error performing boolean operation: {ex.Message}", 
                    "boolean_error"
                );
            }
        }
        
        /// <summary>
        /// Subdivide faces on a ProBuilder mesh
        /// </summary>
        private JObject SubdivideFaces(JObject parameters)
        {
            try
            {
                // Extract parameters
                int instanceId = parameters["instanceId"]?.ToObject<int>() ?? 0;
                int subdivisions = parameters["subdivisions"]?.ToObject<int>() ?? 1;
                
                // Find the GameObject
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (gameObject == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} not found", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder mesh
                ProBuilderMesh mesh = gameObject.GetComponent<ProBuilderMesh>();
                if (mesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} is not a ProBuilder mesh", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Get all faces
                IList<Face> faces = mesh.faces;
                
                // Subdivide the faces
                mesh.Subdivide(faces, subdivisions);
                
                // Rebuild the mesh
                mesh.ToMesh();
                mesh.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully subdivided faces on {gameObject.name}"
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error subdividing faces: {ex.Message}", 
                    "subdivide_error"
                );
            }
        }
        
        /// <summary>
        /// Triangulate faces on a ProBuilder mesh
        /// </summary>
        private JObject TriangulateFaces(JObject parameters)
        {
            try
            {
                // Extract parameters
                int instanceId = parameters["instanceId"]?.ToObject<int>() ?? 0;
                
                // Find the GameObject
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                if (gameObject == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} not found", 
                        "not_found_error"
                    );
                }
                
                // Get the ProBuilder mesh
                ProBuilderMesh mesh = gameObject.GetComponent<ProBuilderMesh>();
                if (mesh == null)
                {
                    return TheBifrostSocketHandler.CreateErrorResponse(
                        $"GameObject with instance ID {instanceId} is not a ProBuilder mesh", 
                        "not_probuilder_mesh"
                    );
                }
                
                // Get all faces
                IList<Face> faces = mesh.faces;
                
                // Triangulate the faces
                mesh.ToMesh();
                mesh.Refresh();
                
                // Return success
                return new JObject
                {
                    ["success"] = true,
                    ["type"] = "text",
                    ["message"] = $"Successfully triangulated faces on {gameObject.name}"
                };
            }
            catch (Exception ex)
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    $"Error triangulating faces: {ex.Message}", 
                    "triangulate_error"
                );
            }
        }
#endif

        /// <summary>
        /// Extract a Vector3 from a JToken
        /// </summary>
        private Vector3 ExtractVector3(JToken token, Vector3 defaultValue)
        {
            if (token == null)
            {
                return defaultValue;
            }
            
            try
            {
                JObject vector = token as JObject;
                if (vector == null)
                {
                    return defaultValue;
                }
                
                return new Vector3(
                    vector["x"]?.ToObject<float>() ?? defaultValue.x,
                    vector["y"]?.ToObject<float>() ?? defaultValue.y,
                    vector["z"]?.ToObject<float>() ?? defaultValue.z
                );
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}