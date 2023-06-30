using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAutoScroll : MonoBehaviour
{
    [SerializeField] private RectTransform _viewportRectTransform;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _transitionDuration = 0.2f;

    private TransitionHelper _transitionHelper = new TransitionHelper();


    void Update()
    {
        if (_transitionHelper.InProgress == true)
        {
            _transitionHelper.Update();
            _content.transform.localPosition = _transitionHelper.PosCurrent;
        }

    }

    public void HandleOnSelectChange(GameObject gObj)
    {
        float viewportTopBorderY = GetBorderTopYLocal(_viewportRectTransform.gameObject);
        float viewportBottomBorderY = GetBorderBottomYLocal(_viewportRectTransform.gameObject);
        //Top
        float targetTopBorderY = GetBorderTopYRelative(gObj);
        float targetTopYWithViewportOffset = targetTopBorderY + viewportTopBorderY;
        //Bottom
        float targetBottomBorderY = GetBorderBottomYRelative(gObj);
        float targetBottomYWithViewportOffset = targetBottomBorderY - viewportBottomBorderY;
        //Top Difference
        float topDiff = targetTopYWithViewportOffset - viewportTopBorderY;
        if (topDiff > 0f)
        {
            MoveContentObjectYByAmount((topDiff * 100f) + GetVerticalLayoutGroup().padding.top);
        }
        //Bottom Difference
        float bottomDiff = targetBottomYWithViewportOffset - viewportBottomBorderY;
        if (bottomDiff < 0f)
        {
            MoveContentObjectYByAmount((bottomDiff * 100f) - GetVerticalLayoutGroup().padding.bottom);
        }

    }

    private float GetBorderTopYLocal(GameObject gObj)
    {
        Vector3 pos = gObj.transform.localPosition / 100f;
        return pos.y;
    }

    private float GetBorderBottomYLocal(GameObject gObj)
    {
        Vector2 rectSize = gObj.GetComponent<RectTransform>().rect.size * 0.01f;
        Vector3 pos = gObj.transform.localPosition / 100f;
        pos.y -= rectSize.y;

        return pos.y;
    }

    private float GetBorderTopYRelative(GameObject gObj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderUpYLocal = GetBorderTopYLocal(gObj);
        float targetBorderUpYRelative = targetBorderUpYLocal + contentY;

        return targetBorderUpYRelative;
    }


    private float GetBorderBottomYRelative(GameObject gObj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderBottomYLocal = GetBorderBottomYLocal(gObj);
        float targetBorderBottomYRelative = targetBorderBottomYLocal + contentY;

        return targetBorderBottomYRelative;
    }

    private void MoveContentObjectYByAmount(float amount)
    {
        Vector2 posScrollFrom = _content.transform.localPosition;
        Vector2 posScrollTo = posScrollFrom;
        posScrollTo.y -= amount;
        _transitionHelper.TransitionPositionFromTo(posScrollFrom, posScrollTo, _transitionDuration);
    }

    private VerticalLayoutGroup GetVerticalLayoutGroup()
    {
        VerticalLayoutGroup verticalLayoutGroup = _content.GetComponent<VerticalLayoutGroup>();
        return verticalLayoutGroup;
    }



    private class TransitionHelper
    {
        private float _duration = 0f;   //the total time that this transition completes in
        private float _timeElapsed = 0f;   //keep track of time
        private float _progress = 0f;   //total progress from start to finish

        private bool _inProgress = false;

        private Vector2 _posCurrent;
        private Vector2 _posFrom;
        private Vector2 _posTo;

        public bool InProgress { get => _inProgress; }

        public Vector2 PosCurrent { get => _posCurrent; }

        public void Update()
        {
            Tick();
            CalculatePosition();
        }

        private void Clear()
        {
            _duration = 0f;
            _timeElapsed = 0f;
            _progress = 0f;

            _inProgress = false;

        }

        public void TransitionPositionFromTo(Vector2 posFrom, Vector2 posTo, float duration)
        {
            Clear();

            _posFrom = posFrom;
            _posTo = posTo;
            _duration = duration;

            _inProgress = true;

        }


        private void CalculatePosition()
        {
            _posCurrent.x = Mathf.Lerp(_posFrom.x, _posTo.x, _progress);
            _posCurrent.y = Mathf.Lerp(_posFrom.y, _posTo.y, _progress);
        }

        private void Tick()
        {
            if (_inProgress == false)
            {
                return;
            }

            _timeElapsed += Time.deltaTime;
            _progress = _timeElapsed / _duration;

            if (_progress > 1f)
            {
                _progress = 1f;
            }

            if (_progress >= 1f)
            {
                TransitionComplete();
            }
        }

        private void TransitionComplete()
        {
            _inProgress = false;
        }

    }
}
