using UnityEngine;

using static GameConstants;


static class Utils {
    static public Vector3 serverFieldCoordsToUnityVector3(float x, float y, float z) {
        return new Vector3(GameConstants.fieldWidth - x, z, GameConstants.fieldHeight - y);
    }

    static public Point unityFieldPointToServerPoint(Vector3 p) {
        Point returnedPoint = new Point();
        returnedPoint.x = GameConstants.fieldWidth - p.x;
        returnedPoint.y = GameConstants.fieldHeight - p.z;
        return returnedPoint;
    }

}
