using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class visualJoyStick : MonoBehaviour
{
    public bool touch;
    public Vector2 joyStickVec;
    // Use this for initialization
    public string hitUIObjectName = "";
    GameObject hitUIObject;
    void Start()
    {
        touch = false;
        joyStickVec = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                hitUIObject = EventSystem.current.currentSelectedGameObject;
                if (hitUIObject)
                {
                    hitUIObjectName = hitUIObject.name;
                }
                else
                {
                    hitUIObjectName = "";
                }
            }

            if (hitUIObjectName == this.name)
            {
                Rect _rect = hitUIObject.GetComponentInParent<RectTransform>().rect;
                Vector2 temp;
                temp.x = Input.mousePosition.x - hitUIObject.transform.position.x + _rect.width * 0.5f;
                temp.y = Input.mousePosition.y - hitUIObject.transform.position.y + _rect.height * 0.5f;
                Vector2 imageScale = hitUIObject.GetComponent<RectTransform>().localScale;
                Sprite _sprite = hitUIObject.GetComponent<UnityEngine.UI.Image>().sprite;
                Color UIObjectRGB;
                UIObjectRGB = _sprite.texture.GetPixel(Mathf.FloorToInt(temp.x * _sprite.texture.width / (_rect.width * imageScale.x)), Mathf.FloorToInt(temp.y * _sprite.texture.height / (_rect.height * imageScale.y)));

                //取得使用者滑鼠點擊處的Alpha值(為了不規則的按鈕)
                if (UIObjectRGB.a != 0
                    && Vector2.Distance(Input.mousePosition, hitUIObject.transform.position) < _rect.width * 0.5)
                {
                    touch = true;
                    transform.Find("stick").transform.position = Input.mousePosition;
                }
                else if (touch)
                {
                    //如果拖拉滑鼠盤脫離搖桿盤的範圍，取得圓的交點
                    Vector2 a = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector2 b = new Vector2(hitUIObject.transform.position.x, hitUIObject.transform.position.y);
                    Vector3 c = new Vector3(hitUIObject.transform.position.x, hitUIObject.transform.position.y, _rect.width * 0.5f);
                    Vector2 x = getIntersections(a.x, a.y, b.x, b.y, c.x, c.y, c.z);
                    GameObject.Find("stick").transform.position = new Vector3(x.x, x.y, 0);
                }
                joyStickVec = GameObject.Find("stick").transform.position - this.transform.position;
                joyStickVec.x /= 50;
                joyStickVec.y /= 50;
            }
        }
        else
        {
            joyStickVec = Vector2.zero;
            hitUIObjectName = "";
            touch = false;
            GameObject.Find("stick").transform.position = this.transform.position;
        }
    }

    //取得交點用
    Vector2 getIntersections(float ax, float ay, float bx, float by, float cx, float cy, float cz)
    {
        float[] a = { ax, ay }, b = { bx, by }, c = { cx, cy, cz };
        // Calculate the euclidean distance between a & b
        float eDistAtoB = Mathf.Sqrt(Mathf.Pow(b[0] - a[0], 2) + Mathf.Pow(b[1] - a[1], 2));

        // compute the direction vector d from a to b
        float[] d = {
            (b[0] - a[0]) / eDistAtoB,
            (b[1] - a[1]) / eDistAtoB
        };

        // Now the line equation is x = dx*t + ax, y = dy*t + ay with 0 <= t <= 1.

        // compute the value t of the closest point to the circle center (cx, cy)
        var t = (d[0] * (c[0] - a[0])) + (d[1] * (c[1] - a[1]));

        // compute the coordinates of the point e on line and closest to c
        var ecoords0 = (t * d[0]) + a[0];
        var ecoords1 = (t * d[1]) + a[1];

        // Calculate the euclidean distance between c & e
        var eDistCtoE = Mathf.Sqrt(Mathf.Pow(ecoords0 - c[0], 2) + Mathf.Pow(ecoords1 - c[1], 2));

        // test if the line intersects the circle
        if (eDistCtoE < c[2])
        {
            // compute distance from t to circle intersection point
            var dt = Mathf.Sqrt(Mathf.Pow(c[2], 2) - Mathf.Pow(eDistCtoE, 2));

            // compute first intersection point
            var fcoords0 = ((t - dt) * d[0]) + a[0];
            var fcoords1 = ((t - dt) * d[1]) + a[1];
            // check if f lies on the line
            //        f.onLine = is_on (a, b, f.coords);

            // compute second intersection point
            var gcoords0 = ((t + dt) * d[0]) + a[0];
            var gcoords1 = ((t + dt) * d[1]) + a[1];
            Vector2 finalAnswer = new Vector2(fcoords0, fcoords1);

            // check if g lies on the line
            return (finalAnswer);

        }

        return (new Vector2());

    }

}
