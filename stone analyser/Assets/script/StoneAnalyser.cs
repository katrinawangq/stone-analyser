using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    //Get all the stones in your scene
    //Loop over all the stones
    //Get longest direction of the stone (find a way to get the longest direction in a mesh)
    //allign the longest direction with the X axis
    //Voxelise the mesh
    //Get the mesh normals
public class StoneAnalyser : MonoBehaviour
{

    MeshFilter[] Stones;
    List<Vector3> stoneMeshNormalGroups;

    public void VoxeliseMesh(MeshFilter Stone)
    {
        //Get bounds of the mesh
        //divide bounds into voxelsize
        //create voxelgrid in the bounds of the mesh
        //Check which voxels are inside the mesh
        //set the voxels active
        Mesh stone = Stone.GetComponent<MeshFilter>().mesh; 
        Vector3 centerPoint = stone.bounds.center; 
        Vector3 extents = stone.bounds.extents;
        Debug.Log(extents);
        float r = Mathf.Min(extents.x, extents.y, extents.z) / 10; 
        for (float x = -extents.x; x <= extents.x; x += r)
        {
            for (float y = -extents.y; y <= extents.y; y += r)
            {
                for (float z = -extents.z; z <= extents.z; z += r)
                {
                    if (IsPointInCollider(Stone.GetComponent<MeshCollider>(), Stone.transform.position + Stone.transform.TransformVector(new Vector3(x, y, z))))
                    {
                        GameObject Voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Voxel.transform.SetParent(Stone.transform);
                        Voxel.transform.localEulerAngles = Vector3.zero;
                        Voxel.transform.localPosition = centerPoint + new Vector3(x, y, z);
                        Voxel.transform.localScale = Vector3.one * r;
                        Destroy(Voxel.GetComponent<Collider>());
                    }
                }
            }
        }
        Stone.GetComponent<MeshRenderer>().enabled = false;
        stoneMeshNormalGroups = GetMeshNormals(stone);
    }

    private List<Vector3> GetMeshNormals(Mesh stone)
    {
        throw new NotImplementedException();
    }


    //Get longest direction of the stone (find a way to get the longest direction in a mesh)
    //allign the longest direction with the X axis
    public void PlaceStoneByLongestDirection(MeshFilter Stone)
    {
        Mesh stone = Stone.GetComponent<MeshFilter>().mesh;
        Vector3 direction = stone.bounds.max - stone.bounds.min;
        Stone.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    // Start is called before the first frame update
    void Start()
    {
        Stones = FindObjectsOfType<MeshFilter>(); 
        for (int i = 0; i < Stones.Length; i++)
        {
            PlaceStoneByLongestDirection(Stones[i]);
        }
        Invoke("InvokeVoxeliseMesh", 2); 
    }
    void InvokeVoxeliseMesh()
    {
        for (int i = 0; i < Stones.Length; i++) 
        {
            VoxeliseMesh(Stones[i]);
        }
    }


    public static bool IsPointInCollider(Collider cld, Vector3 point)
        {

            Vector3 direction = new Vector3(0, 1, 0);

            if (Physics.Raycast(point, direction, Mathf.Infinity) &&
                Physics.Raycast(point, -direction, Mathf.Infinity))
            {
                return true; 
            }

            else return false;
        }


    public List<Vector3> GetMeshNormals(Mesh stone, Vector3 firstNormal)
    {
        //take a random face in the mesh
        // Get its normal
        // make a new normal group with the face normal

        //loop untill entire mesh is checked
        //Get next face using Breadth First Search
        //if angle between new face normal and group normal < tolerance
        //add the face to the normal group
        //get the average normal of all faces in this normal group
        //if angle between new face normal and group normal > tolerance
        //Make a new normal group
        //Set next face as checked


        List<Vector3> normalGroup = new List<Vector3>();
        float tolerance = 0.1f;
        Vector3 randomNormal = stone.normals[0];
        normalGroup.Add(firstNormal);
        Vector3 groupNormal = randomNormal;

        for (int i = 0; i < stone.normals.Length; i++)
        {
            if (Vector3.Angle(groupNormal, stone.normals[i]) < tolerance)
            {
                normalGroup.Add(stone.normals[i]);
                groupNormal = normalGroup.Average();
            }
            else
            {
                normalGroup = new List<Vector3>();
            }
        }

        return normalGroup;

    }
}
