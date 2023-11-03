
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class SGTEventGameObject : UnityEvent<GameObject> { }

[System.Serializable]
public class SGTEventInt : UnityEvent<int> { }


[System.Serializable]
public class SGTEventFloat : UnityEvent<float> { }


[System.Serializable]
public class SGTEventTransform : UnityEvent<Transform> { }


[System.Serializable]
public class SGTEventFloatQuaternion : UnityEvent<float,Quaternion,Vector3> { }

[System.Serializable]
public class SGTEventFloatVector3Vector3 : UnityEvent<float, Vector3, Vector3> { }

[System.Serializable]
public class SGTEventVector3 : UnityEvent<Vector3> { }


[System.Serializable]
public class SGTEventCollision : UnityEvent<Vector3, GameObject,Vector3> { }

[System.Serializable]
public class SGTEventCollider : UnityEvent<Collider> { }

[System.Serializable]
public class SGTEventColliderArray : UnityEvent<Collider[]> { }

[System.Serializable]
public class SGTEventString : UnityEvent<string> { }



[System.Serializable]
public class SGTEventDamageAI : UnityEvent<GameObject , Quaternion , Vector3 , Vector3 > { }

[System.Serializable]
public class SGTEventThreeFloat : UnityEvent<float, float, float> { }

[System.Serializable]
public class SGTEventBool : UnityEvent<bool> { }

[System.Serializable]
public class SGTEventVector3Vector3 : UnityEvent<Vector3, Vector3> { }
