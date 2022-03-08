using UnityEngine;


static class Utils {
    static float fieldWidth = 30F;
    static float fieledHeight = 15F;

    static public Vector3 serverFieldCoordsToUnityVector3(float x, float y, float z) {
        return new Vector3(fieldWidth - x, z, fieledHeight - y);
    }

    static public Point unityFieldPointToServerPoint(Vector3 p) {
        var returnedPoint = new Point();
        returnedPoint.x = fieldWidth - p.x;
        returnedPoint.y = fieledHeight - p.z;
        return returnedPoint;
    }

}
