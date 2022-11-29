using System.Numerics;

namespace Veldrid.Maui.Controls.Base
{
    public class SimpleCamera : BaseCamera
    {
        private Vector2 _previousMouseTotalDelta;
        private Vector2 _currentMouseTotalDelta;
        public void UpdatePanPoint(float x, float y)
        {
            if (_previousMouseTotalDelta == default)
                _currentMouseTotalDelta = _previousMouseTotalDelta = new Vector2(x, y);
            else
            {
                _previousMouseTotalDelta = _currentMouseTotalDelta;
                _currentMouseTotalDelta = new Vector2(x, y);
            }
        }

        public override void Update(float deltaSeconds)
        {
            Vector2 mouseDelta = _currentMouseTotalDelta - _previousMouseTotalDelta;

            Yaw += -mouseDelta.X * 0.01f;
            Pitch += -mouseDelta.Y * 0.01f;
            Pitch = Clamp(Pitch, -1.55f, 1.55f);

            UpdateViewMatrix();
        }
    }
}
