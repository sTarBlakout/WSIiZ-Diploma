﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The namespace used by SimplePathFinding2D 
namespace SimplePF2D{

    // A class that acts as a container for the path that is generated by the A* path finding algorithm
    // Path has a list of points as well as an internal pointer that tracks which point the path is on.
    // The path can be traversed backward and forward.
    public class Path {

        private SimplePathFinding2D pf; // An object that does overarching handling of a specific grid's pathfinding.
        private List<Vector3Int> pathpoints; // A list of vectors that are Tile coordinates that represent each point in the path.
        private AStarSearch aStarSearch; // An object used to perform an A* search.
        private Vector3 startpos; // The start and and end position of the path (in world coordinates).
        private Vector3 endpos;
        private bool searchWithDiagonals; // Specifies whether this path searches using diagonals. Hexagonal layouts do not use diagonals.
        private bool isdynamicflag; // Is a static or dynamic path. If the path is blocked, dynamic paths regenerate the path from the point of blockage. 
                                    // However, static paths generate a path from the initially specified start position of the path.
        private int internalindex = 0; // The internal pointer that keep tracks of the point in the path point list this path is on.
        private static bool init = false;

        // This needs to be created with a valid SimplePathFinding2D object. Not doing so will cause issues.
        public Path(SimplePathFinding2D newpf){
            pf = newpf;
            pathpoints = new List<Vector3Int>();
            aStarSearch = new AStarSearch(pf);
        }

        // Resets and clears a path. Setting the index to the start of the path (which is zero).
        public void Reset(){
            pathpoints.Clear();
            internalindex = 0;
            endpos = Vector3.zero;
            aStarSearch.hasFailedFlag = false;
        }

        // Manually sets the current path point index.
        public void SetPathPointIndex(int index){
           internalindex = Mathf.Clamp(index, 0, pathpoints.Count);
        }

        // Sets the current index of the path to the start.
        public void SetPathStart() {
            internalindex = 0;
        }

        // Sets the current index of the path to the end.
        public void SetPathEnd() {
            internalindex = pathpoints.Count - 1;
        }

        // Returns the list of path points.
        public List<Vector3Int> GetPathPointList(){
            return pathpoints;
        }

        // Returns the internal index in the path point list this path is on.
        public int GetInternalPathPointIndex(){
            return internalindex;
        }

        // Returns the point the path's internal pointer is currently pointing at.
        public Vector3Int GetPathPoint(){
            if (internalindex < 0 || internalindex >= pathpoints.Count){
                return Vector3Int.zero;
            }
            return pathpoints[internalindex];
        }

        // Returns the point the passed in index points at. Does a range check.
        public Vector3Int GetPathPoint(int index){
            if (index < 0 || index >= pathpoints.Count) {
                return Vector3Int.zero;
            }
            return pathpoints[index];
        }

        // Returns the world position of a path point pointed to by the internal path pointer
        public Vector3 GetPathPointWorld(){
            return pf.NavToWorld(GetPathPoint(internalindex));
        }

        // Returns the world position of a path point pointed to by a custom index
        public Vector3 GetPathPointWorld(int index){
            return pf.NavToWorld(GetPathPoint(index));
        }

        // Returns true if the path has points in its path point list and it's A* object hasn't failed.
        // Basically signifies that this path is ready to rumble.
        public bool IsGenerated() {
            return pathpoints.Count != 0 && !aStarSearch.hasFailedFlag;
        }
        
        public bool IsGenerationFailed() {
            return aStarSearch.hasFailedFlag;
        }

        // Gets the next point in the path point list based on the passed in index.
        // This point is returned by reference as a world position (nextPoint). Returns true if there is a valid next point.
        // If it reaches the end or the path has failed it returns false.
        // If the next point in the point list is blocked a new path is generated.
        public bool GetNextPointIndex(ref Vector3 nextPoint, ref int index) {

            // We couldn't generate a path OR we are still waiting for the path to be generated.
            if (!IsGenerated()){
                return false;
            }

            index++;
            if (index < pathpoints.Count) {
                // Check to see if this node is blocked at this point.
                Vector3 worldpos = GetPathPointWorld(index);
                NavNode node = pf.GetNode(worldpos);
                if (node.IsBlocked()){ // Create a new path if the previous path is no longe usuable.

                    if (isdynamicflag){
                        CreatePath(GetPathPointWorld(index - 1), endpos, searchWithDiagonals, true);
                    } else{
                        CreatePath(startpos, endpos, searchWithDiagonals, false);
                    }
                    return false;
                } else { // We successfully found the next point.
                    nextPoint = worldpos;
                    return true;
                }
            }
            // We reached the end of our path.
            return false;
        }

        // Returns the next point in the list based on the internal pointer of the path.
        public bool GetNextPoint(ref Vector3 nextPoint){
            return GetNextPointIndex(ref nextPoint, ref internalindex);
        }

        // Gets the previous point in the path point list based on the passed in index.
        // This point is returned by reference as a world position (previousPoint). Returns true if there is a valid next point.
        // If it reaches the end or the path has failed it returns false.
        // If the next point in the point list is blocked a new path is generated.
        public bool GetPreviousPointIndex(ref Vector3 previousPoint, ref int index){

            // We couldn't generate a path OR we are still waiting for a path.
            if (!IsGenerated()){
                return false;
            }

            index--;
            if (index >= 0){
                // Check to see if this node is blocked at this point.
                Vector3 worldpos = GetPathPointWorld(index);
                NavNode node = pf.GetNode(worldpos);
                if (node.IsBlocked()) { // Create a new path if the previous path is no longer usuable.

                    if (isdynamicflag){
                        CreatePath(GetPathPointWorld(index + 1), startpos, searchWithDiagonals, true);
                    } else {
                        CreatePath(endpos, startpos, searchWithDiagonals, false);
                    }

                    return false;
                } else {
                    previousPoint = worldpos;
                    return true;
                }
            }

            return false;
        }

        // Returns the previous point in the list based on the internal pointer of the path.
        public bool GetPreviousPoint(ref Vector3 previousPoint) {
            return GetPreviousPointIndex(ref previousPoint, ref internalindex);
        }

        // Create a path from one world position to another. (in 2D)
        // Can be configured to search using diagonals.
        // Dynamic path's recalculate the path when encountering obstacles from the obstacles position.
        // Static path's recalculate the path when encountering obstacles from the paths original start position.
        public bool CreatePath(Vector3 startPosWorld, Vector3 endPosWorld, bool searchUsesDiagonals = true, bool isdynamic = true, int overrideiterations = 0)
        {
            DebugClearMarkers();

            // Resets this path if there already is one.
            if (IsGenerated()){
                Reset();
            }

            startpos = startPosWorld;
            endpos = endPosWorld;
            Vector3Int startNavPos = pf.WorldToNav(startPosWorld);
            Vector3Int endNavPos = pf.WorldToNav(endPosWorld);
            searchWithDiagonals = searchUsesDiagonals;
            isdynamicflag = isdynamic;
            // The A* object only uses Nav grid coordinates.
            return aStarSearch.StartPath(startNavPos, endNavPos, pathpoints, searchWithDiagonals, overrideiterations);
        }

        // Create a path from one tile coordinate to another.
        public bool CreatePath(Vector3Int startTile, Vector3Int endTile, bool searchUsesDiagonals = true, bool isdynamic = true, int overrideiterations = 0){
            if (!init) {
                return false;
            }

            // Resets this path if there already is one.
            if (IsGenerated()){
                Reset();
            }

            Vector3Int startPosNav = pf.TileToNav(startTile);
            Vector3Int endPosNav = pf.TileToNav(endTile);
            startpos = pf.NavToWorld(startPosNav);
            endpos = pf.NavToWorld(endPosNav);
            searchWithDiagonals = searchUsesDiagonals;
            isdynamicflag = isdynamic;
            return aStarSearch.StartPath(startPosNav, endPosNav, pathpoints, searchWithDiagonals, overrideiterations);
        }

        // When debug markers are active, clears them for this path.
        public void DebugClearMarkers(){
            pf.DebugClearPathMarker();
        }
    }
}

