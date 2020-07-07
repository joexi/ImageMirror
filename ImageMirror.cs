using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageMirror : BaseMeshEffect
{
    public enum MirrorType
    {
        Horizontal,
        Vertical,
        Quater,
    }

    protected const int AxisX = 0;
    protected const int AxisY = 1;

    [SerializeField]
    private MirrorType type;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        Image img = graphic as Image;
        if (null == img) return;

        if (img.type == Image.Type.Simple)
        {
            _SimpleMirror(vh);
        }
    }

    private void _SimpleMirror(VertexHelper vh)
    {
        Rect rect = graphic.GetPixelAdjustedRect();
        ShrinkVert(vh, rect);

        Vector2 doubleCenter = rect.center * 2;
        switch (type)
        {
            case MirrorType.Horizontal:
                SimpleMirrorHor(vh, doubleCenter.x);
                break;

            case MirrorType.Vertical:
                SimpleMirrorVer(vh, doubleCenter.y);
                break;

            case MirrorType.Quater:
                SimpleMirrorQuat(vh, doubleCenter);
                break;
        }
    }

    protected void SimpleMirrorHor(VertexHelper vh, float doubleX)
    {
        AddMirrorVert(vh, 0, AxisX, doubleX);
        AddMirrorVert(vh, 1, AxisX, doubleX);
        vh.AddTriangle(2, 4, 3);
        vh.AddTriangle(2, 5, 4);
    }

    protected void SimpleMirrorVer(VertexHelper vh, float doubleY)
    {
        AddMirrorVert(vh, 0, AxisY, doubleY);
        AddMirrorVert(vh, 3, AxisY, doubleY);

        vh.AddTriangle(2, 1, 4);
        vh.AddTriangle(2, 4, 5);
    }

    protected void SimpleMirrorQuat(VertexHelper vh, Vector2 doubleCenter)
    {
        AddMirrorVert(vh, 0, AxisX, doubleCenter.x);
        AddMirrorVert(vh, 1, AxisX, doubleCenter.x);
        vh.AddTriangle(2, 4, 3);
        vh.AddTriangle(2, 5, 4);

        AddMirrorVert(vh, 0, AxisY, doubleCenter.y);
        AddMirrorVert(vh, 3, AxisY, doubleCenter.y);
        AddMirrorVert(vh, 4, AxisY, doubleCenter.y); 
        vh.AddTriangle(7, 1, 6);
        vh.AddTriangle(7, 2, 1);
        vh.AddTriangle(7, 5, 2);
        vh.AddTriangle(7, 8, 5);
    }

    protected static void AddMirrorVert(VertexHelper vh, int srcVertIdx, int axis, float doubleCenter)
    {
        UIVertex vert = UIVertex.simpleVert;
        vh.PopulateUIVertex(ref vert, srcVertIdx);
        Vector3 pos = vert.position;
        pos[axis] = doubleCenter - pos[axis];
        vert.position = pos;
        vh.AddVert(vert);
    }

    protected void ShrinkVert(VertexHelper vh, Rect rect)
    {
        int count = vh.currentVertCount;

        UIVertex vert = UIVertex.simpleVert;
        for (int i = 0; i < count; ++i)
        {
            vh.PopulateUIVertex(ref vert, i);
            Vector3 pos = vert.position;
            if (MirrorType.Horizontal == type || MirrorType.Quater == type)
            {
                pos.x = (rect.x + pos.x) * 0.5f;
            }
            if (MirrorType.Vertical == type || MirrorType.Quater == type)
            {
                pos.y = (rect.y + pos.y) * 0.5f;
            }
            vert.position = pos;
            vh.SetUIVertex(vert, i);
        }
    }

    private RectTransform trans;
    public RectTransform rectTransform
    {
        get
        {
            if (null == trans)
            {
                trans = GetComponent<RectTransform>();
            }
            return trans;
        }
    }

    public void SetNativeSize()
    {
        Image img = graphic as Image;
        if (null == img) return;

        Sprite sprite = img.overrideSprite;
        if (null == sprite) return;

        float w = sprite.rect.width / img.pixelsPerUnit;
        float h = sprite.rect.height / img.pixelsPerUnit;
        rectTransform.anchorMax = rectTransform.anchorMin;
        switch (type)
        {
            case MirrorType.Horizontal:
                rectTransform.sizeDelta = new Vector2(w * 2, h);
                break;
            case MirrorType.Vertical:
                rectTransform.sizeDelta = new Vector2(w, h * 2);
                break;
            case MirrorType.Quater:
                rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                break;
        }
        img.SetVerticesDirty();
    }
}
