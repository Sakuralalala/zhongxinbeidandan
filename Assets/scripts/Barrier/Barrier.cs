using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.ComponentModel.Design;
using System.Runtime.Serialization.Formatters;
using System.Xml.Linq;

using System.IO;
using NUnit.Framework;

public class Barrier : MonoBehaviour
{
	public LineRenderer debugLine;
	public PolygonCollider2D collider;
	public float R = 2;
	List<Vector3> vertices;

	public void Init(int[] starsIDs){
		StartCoroutine (Calculate (starsIDs));
	}

	public IEnumerator Calculate(int[] starsID){
		List<Vector3> starsPositions = new List<Vector3>();
		vertices = new List<Vector3> ();
		foreach (int ID in starsID) {
			starsPositions.Add (TestPlayer.getStar (ID).transform.localPosition);
		}

		//----------------------------------------找出凸多边形--------------------------------------------
		Vector3 current = new Vector3 (),next = new Vector3(9999,9999,9999),start;
		Vector3 currentLine = new Vector3 (0, 1, 0);
		//--------找出起始点
		float maxX= starsPositions [0].x;
		for (int i = 0; i < starsPositions.Count; i++) {
			float tempX = starsPositions [i].x;
			if (tempX > maxX) {
				maxX = tempX;
			} 
		}
		float minY = 99999;
		List<Vector3> temp = new List<Vector3> ();
		for (int i = 0; i < starsPositions.Count; i++) {
			if (starsPositions [i].x == maxX) {
				temp.Add (starsPositions [i]);
				if (starsPositions [i].y < minY) {
					minY = starsPositions [i].y;
					current = starsPositions[i];
				}
			}
		}
		start = current;
		System.Collections.Generic.IComparer<Vector3> comp = new PointCompareDis (start); 
		temp.Sort (comp);
		for (int i = 0; i < temp.Count; i++) {
			vertices.Add (temp[i]);
		}
		current = temp [temp.Count - 1];
		bool startMask = true;
		//--------开始循环
		while ((!start.Equals (current))||startMask) {yield return null;if(startMask) startMask = false;
			//找到最小角
			float minAngle = 9999;
			for (int i = 0; i < starsPositions.Count; i++) {
				if (starsPositions [i].Equals (current))
					continue;
				Vector3 tempLine = starsPositions [i] - current;
				float tempAngle = Angle (currentLine, tempLine);
				if (tempAngle< minAngle&&tempAngle!=0) {
					minAngle = tempAngle;
				}
			}
			//找到最小角方向上的所有点
			List<Vector3> PointsOnLine = new List<Vector3> ();
			for (int i = 0; i < starsPositions.Count; i++)
				if (Angle (currentLine, starsPositions [i] - current) == minAngle)
					PointsOnLine.Add (starsPositions [i]);
			PointsOnLine.Sort ((IComparer<Vector3>)new PointCompareDis (current));
			//依距离将此直线上的所有点按顺序加入顶点数组，更新当前点为最远点和当前方向为最小角方向
			for (int i = 0; i < PointsOnLine.Count; i++) {
				vertices.Add (PointsOnLine [i]);
			}
			currentLine = PointsOnLine[0]-current;
			current = PointsOnLine [PointsOnLine.Count - 1];
		}
		//----------------------------------------找出凸多边形--------------------------------------------

		//----------------------------------------对边的坐标修正--------------------------------------------
		vertices.RemoveAt (vertices.Count-1);
		for (int i = 0; i < vertices.Count; i+=3) {
			int nextVertex = (i+1)%vertices.Count;
			int prevVertex = (i - 1 + vertices.Count) % vertices.Count;
			Vector3 leftEdge = vertices[prevVertex] - vertices [i];
			Vector3 rightEdge = vertices [nextVertex] - vertices [i];
			Vector3 right = new Vector3(rightEdge.y,-rightEdge.x);
			Vector3 left = new Vector3 (-leftEdge.y, +leftEdge.x);
			Vector3 rightPoint = vertices [i] + right.normalized * R;
			Vector3 leftPoint = vertices [i] + left.normalized * R;
			vertices.Insert (i,leftPoint);
			vertices.Insert (i+2,rightPoint);
			yield return null;
		}
		Vector3 head = vertices [0];
		vertices.Add (head);
		//----------------------------------------对边的坐标修正--------------------------------------------

		// 多边形Collider
		List<Vector2> colliderPoint = new List<Vector2>();
		foreach (Vector3 point in vertices) {
			colliderPoint.Add (point);
		}
		AddPolygonColliderElement (colliderPoint.ToArray ());

		//DEBUG
		//用找到的顶点画多边形
//		debugLine.positionCount = vertices.Count;
//		debugLine.SetPositions (vertices.ToArray ());

		Render ();
	}

	public void Render(){
		// 上面得到的是逆时针方向绕点，根据unity中mesh的要求，必须为逆时针，所以反向
		vertices.Reverse ();	
		//----------------------------------------分割三角形--------------------------------------------
		int vertexNumber = vertices.Count-1;
		int sunkenVertexNumber = vertexNumber / 3;
		List<int> indexes = new List<int> (3*(vertexNumber-1));
		//-------------内部凹点组成的多边形网格
		for (int i = 0; i < sunkenVertexNumber-2; i++) {
			indexes.Add (2);
			indexes.Add (i*3+5);
			indexes.Add (i*3+8);
		}
		//-------------外部凸点组成的长方形网格
		for (int i = 0; i < sunkenVertexNumber-1; i++) {
			indexes.Add (i*3+2);
			indexes.Add (i*3+3);
			indexes.Add (i*3+4);

			indexes.Add (i*3+2);
			indexes.Add (i*3+4);
			indexes.Add (i*3+5);
		}
		indexes.Add (vertices.Count-2);
		indexes.Add (0);
		indexes.Add (1);

		indexes.Add (vertices.Count-2);
		indexes.Add (1);
		indexes.Add (2);
		//----------------------------------------分割三角形--------------------------------------------

		//----------------------------------------添加圆角--------------------------------------------
		float dtheta =10f;
		for (int i = 0; i < sunkenVertexNumber; i++) {
			Vector3 origin = vertices [3 * i + 2];
			Vector3 left = vertices [3 * i+1];
			Vector3 right = vertices [3 * i + 3];
			Vector3 current = left - origin;
			int baseCount = vertices.Count;

			current = Rotate (current, -dtheta);
			Vector3 result = current + origin;
			vertices.Add (result);
			indexes.Add (3 * i + 2);																														// 中间凹点
			indexes.Add (3 * i+1); 																														// 左上角点
			indexes.Add (vertices.Count-1);																									// 新得到的弧线点

			while(Angle (right - origin,current)>dtheta){
				current = Rotate (current, -dtheta);
				result = current + origin;
				vertices.Add (result);
				indexes.Add (3 * i + 2);																													// 中间凹点
				indexes.Add (vertices.Count-2);																								// 上一个弧线点
				indexes.Add (vertices.Count-1);																								// 新得到的弧线点
			}

 			indexes.Add (3 * i + 2);																														// 中间凹点
			indexes.Add (vertices.Count-1);																									// 新的到的弧线点
			indexes.Add (3 * i+3); 																														// 右上角点

			// 多边形Collider
			List<Vector2> colliderPoint = new List<Vector2> ();
			colliderPoint.Add (vertices[3 * i + 2]);																						// 左上角点
			for(int k = baseCount;k<vertices.Count;k++){
				colliderPoint.Add (vertices[k]);																									// 中间弧线点
			}
			
			colliderPoint.Add (vertices[3 * i + 3]);																						// 右上角点

			AddPolygonColliderElement (colliderPoint.ToArray ());
		}
		//----------------------------------------添加圆角--------------------------------------------

		//----------------------------------------添加网格然后渲染--------------------------------------------
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter> ().mesh = mesh;
		mesh.vertices = this.vertices.ToArray ();
		mesh.triangles = indexes.ToArray ();
		mesh.RecalculateNormals();  
		mesh.RecalculateBounds(); 
		//----------------------------------------添加网格然后渲染--------------------------------------------
	}

	//----------------------------------------添加多边形Collider--------------------------------------------
	private void AddPolygonColliderElement(Vector2[] points){
		collider.pathCount = collider.pathCount + 1;
		collider.SetPath (collider.pathCount-1,points);
	}
	//----------------------------------------添加多边形Collider--------------------------------------------

	private float Angle(Vector3 start,Vector3 end){
		float sign = Vector3.Dot (Vector3.Cross (start, end), new Vector3 (0,0,-1));
		if (sign > 0)
			return -1*Vector3.Angle (start, end);
		else
			return Vector3.Angle (start, end);
	}

	private Vector3 Rotate(Vector3 current,float theta){
		theta = Mathf.PI * theta / 180;
		return  new Vector3 (current.x * Mathf.Cos (theta) - current.y * Mathf.Sin (theta), current.y * Mathf.Cos (theta) + current.x * Mathf.Sin (theta));
	}

	//DEBUG
	public GameObject[] stars;
	public GameObject meteorolite;
	void Start(){
		int[] IDs = new int[]{0,1,2,3,4,5,6};
		StartCoroutine (Calculate (IDs));
		InvokeRepeating ("SpawnMeteorolite",0,5);
	}
	void SpawnMeteorolite(){
		GameObject me = Instantiate (meteorolite);
		//		me.GetComponent<Rigidbody> ().velocity = new Vector3 (3,0,0);
		me.GetComponent<Rigidbody2D> ().velocity = new Vector3 (3,0,0);
		me.transform.position = new Vector3 (-20, 5.02f, -1);
	}

	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.name == "meteorolite") {
			Collider2D collider = collision.gameObject.GetComponent<Collider2D> ();
			Debug.Log ("get collision");
		} else {
			Debug.Log ("get collision but not meteorolite");
		}
	}

}

class PointCompareDis : System.Collections.Generic.IComparer<Vector3>{
	Vector3 referencePoint;
	public PointCompareDis(Vector3 _referencePoint){
		this.referencePoint = _referencePoint;
	}
	int IComparer<Vector3>.Compare(Vector3 x,Vector3 y){
		return (int)((x - referencePoint).magnitude - (y - referencePoint).magnitude);
	}
}