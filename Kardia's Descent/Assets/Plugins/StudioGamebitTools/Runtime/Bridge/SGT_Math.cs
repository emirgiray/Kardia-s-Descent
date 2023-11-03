using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
namespace SGT_Tools.Bridge
{


public static class SGT_Math 
{






    //min ve max radius arasında random point verir. merkeze yakın yerde eklemez. Playerin dibinde filan çıkmaz.
    public static  Vector3 RandomBetweenRadius3D(float minRad, float maxRad)
    {
        float diff = maxRad - minRad;
        Vector3 point = Vector3.zero;
        while (point == Vector3.zero)
        {
            point = UnityEngine.Random.insideUnitSphere;
        }
        point = point.normalized * (UnityEngine.Random.value * diff + minRad);
        return point;
    }


    public static Vector3 RandomBetweenRadius2D(float minRad, float maxRad)
    {
        float radius = UnityEngine.Random.Range(minRad, maxRad);
        float angle = UnityEngine.Random.Range(0, 360);

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

        return new Vector3(x, y, 0);
    }

        // X ve Z olarak random pozisyon bilgisini verir.
        public static Vector3 GetPositionAroundObject(Transform center, float radius)
        {
            Vector3 offset = UnityEngine.Random.insideUnitSphere * radius;
            Vector3 pos = offset + center.position;

            Vector3 XZ = new Vector3(pos.x, center.position.y, pos.z);
            return XZ;
        }


        //Navmesh üzerinden random point bulur.
        public static Vector3 RandomNavmeshLocation(float radius, Vector3 Center)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
            randomDirection += Center;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }




        //Üstteki remap nedense çalışmadı ama biryerde kullandığımı da hatırlıyorum o yüzden şimdilik ikincisini yarattım.
        public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }


        //Belirlediğim objedeki child'i buluyor ve atamasını yapıyor.
        public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
        {
            //Author: Isaac Dart, June-13.
            Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }



        private static GameObject _Closest;
        public static GameObject GetClosestFromList(GameObject[] Listeler,Transform FromThisPosition)
        {
            float minimumDistance = Mathf.Infinity;
            
            foreach (GameObject TargetZone in Listeler)
            {
                float distance = Vector3.Distance(FromThisPosition.position, TargetZone.transform.position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;

                    _Closest =TargetZone.gameObject;
             
                    

                }
            }
    
                return _Closest;

            
     
        }


        //You can use with this class SGT_Parabole 
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector2.Lerp(start, end, t);

            return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
        }

        //This will give you random point in forward Direction within angle
        public static Vector3 RandomPointInCircle(Transform trans, float radius, float MinAngle, float MaxAngle)
        {
            float angle = UnityEngine.Random.Range(MinAngle, MaxAngle);

            float rad = angle * Mathf.Deg2Rad;
            Vector3 position = trans.right * Mathf.Sin(rad) + trans.forward * Mathf.Cos(rad);
            return trans.position + position * radius;
        }

        //This will give you random point in forward Direction within angle
        //Not Tested very well
        public static Vector3 RandomBackPointInCircle(Transform trans, float radius, float MinAngle, float MaxAngle)
        {
            float angle = UnityEngine.Random.Range(MinAngle, MaxAngle);

            float rad = angle * Mathf.Deg2Rad;
            Vector3 position = trans.right * Mathf.Sin(rad) + -trans.forward * Mathf.Cos(rad);
            return trans.position + position * radius;
        }


        //Shuffle array inside, It will shuffle all index each other.
        public static GameObject[] ShuffleArray(GameObject[] _Array)
        {
            for (int i = 0; i < _Array.Length; i++)
            {
                int rnd = UnityEngine.Random.Range(0, _Array.Length);
                GameObject tempGO = _Array[rnd];
                _Array[rnd] = _Array[i];
                _Array[i] = tempGO;
            }
            return _Array;
        }

    }
}