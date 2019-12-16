using ElfKingdom;
using System.Collections.Generic;

public class OtherFunc
{
    public static GameObject GetClosestGameObjectToLocationFromGameObjectArray(Game game, GameObject[] objs, Location location)
    {
        if (objs.Length == 0) return null;
        GameObject closest = null;
        foreach (GameObject gameObj in objs)
        {
            if (closest == null)
            {
                if (gameObj != null)
                    closest = gameObj;
            }
            else  if (gameObj != null)
            {
                if (closest.Distance(location) > gameObj.Distance(location))
                    closest = gameObj;
            }
        }
        return closest;
    }
    
    public static Location GetClosestEnemyEntityToLocation(Game game, Location location)
    {
        Castle myCastle = game.GetMyCastle();
        Creature closestCreature = GetClosestEnemyCreatureTo(game, location); // Closest Enemy Creature to Ally Castle
        Elf closestElf = ElfFunc.GetClosestEnemyElfTo(game, location); // Closest Enemy Elf to Ally Castle

        if (closestCreature == null && closestElf == null)
            return null;
        else if (closestCreature == null)
            return closestElf.Location;
        else if (closestElf == null)
            return closestCreature.Location;
        else if (closestCreature.Location.Distance(myCastle.Location) < closestElf.Location.Distance(myCastle.Location))
            return closestCreature.Location;
        else
            return closestElf.Location;
    }
    
    public static Location GetMyClosestEntityToLocation(Game game, Location location)
    {
        Creature closestCreature = GetMyClosestCreatureTo(game, location);
        Elf closestElf = ElfFunc.GetClosestMyElfTo(game, location);

        if (closestCreature == null && closestElf == null)
            return null;
        else if (closestCreature == null)
            return closestElf.Location;
        else if (closestElf == null)
            return closestCreature.Location;
        else if (closestCreature.Location.Distance(location) < closestElf.Location.Distance(location))
            return closestCreature.Location;
        else
            return closestElf.Location;
    }
    

    public static Creature GetClosestEnemyCreatureTo(Game game, Location location)
    {
        Creature[] enemyCreatures = game.GetEnemyCreatures();
        Creature closestCreature = null;
        if (enemyCreatures.Length > 0)
            closestCreature = enemyCreatures[0];

        foreach (Creature creature in enemyCreatures)
        {
            if (creature.Location.Distance(location) < closestCreature.Location.Distance(location))
                closestCreature = creature;
        }

        return closestCreature;
    }
    
    public static Creature GetMyClosestCreatureTo(Game game, Location location)
    {
        Creature[] myCreatures = game.GetMyCreatures();
        Creature closestCreature = null;
        if (myCreatures.Length > 0)
            closestCreature = myCreatures[0];

        foreach (Creature creature in myCreatures)
        {
            if (creature.Location.Distance(location) < closestCreature.Location.Distance(location))
                closestCreature = creature;
        }

        return closestCreature;
    }
    
    public static bool IsObjectGoingToBeInRangeOfLocation(Game game, Location lastTurnLocation, Location currentLocation, Location isGoingToBeHere, int range = 1)
    // return if the object is going to be in the range of the location by the way he is going now
    {
        int distanceLastToIsGoingHere = lastTurnLocation.Distance(isGoingToBeHere);
        Location whereHeIsGoingToEndUp = BetterTowards(game, lastTurnLocation, currentLocation, distanceLastToIsGoingHere);
        if (whereHeIsGoingToEndUp == null)
            return false;
        if (whereHeIsGoingToEndUp.InRange(isGoingToBeHere, range))
            return true;
        return false;
    }
    
    //Needs the entity max speed (creature.MaxSpeed / elf.MaxSpeed)
    //And needs the entity's locations
    public static int TurnsToGetToLocation(Game game, int entityMaxSpeed, Location entityLocation, Location location)
    {
        int entityDistanceFromLocation = entityLocation.Distance(location);
        int turnsRoundedUp = (int)System.Math.Ceiling((double)(entityDistanceFromLocation / entityMaxSpeed));
        return turnsRoundedUp;
    }

    public static Location[] GetAllLocationsThatNeedDefense(Game game, Location[] locationsThatMayNeedDefense, int range)
    {
        if(locationsThatMayNeedDefense.Length == 0)
            return new Location[0];
            
        Stack<Location> locationsThatneedDefenseStack = new Stack<Location>();
        
        foreach(Location location in locationsThatMayNeedDefense)
        {
            foreach(Elf elf in game.GetEnemyLivingElves())
                if(elf.Location.InRange(location, range))
                    locationsThatneedDefenseStack.Push(location);
        }
        
        Location[] locationsThatneedDefenseArray = new Location[locationsThatneedDefenseStack.Count];
        for(int i = 0; i < locationsThatneedDefenseArray.Length; i++)
            locationsThatneedDefenseArray[i] = locationsThatneedDefenseStack.Pop();
        
        return locationsThatneedDefenseArray;
    }

    public static Location BetterTowards(Game game, Location from, Location to, int distance)
    {
        if (from == null || to == null)
            return null;
        if (from.Equals(to))
            return null;
        double colToAdd = ((double)(to.Col) - (double)(from.Col)) / (double)(from.Distance(to));
        double rowToAdd = ((double)(to.Row) - (double)(from.Row)) / (double)(from.Distance(to));
        double finalRowToAdd = rowToAdd * distance;
        double finalColToAdd = colToAdd * distance;
        Location finalLocation = new Location((int)(from.Row + finalRowToAdd), (int)(from.Col + finalColToAdd));
        return finalLocation;
    }

    public static bool WillTheObjectWillBeInTheMap(Game game, Location objectLocation, int objectSize)
    {
        if (objectLocation == null)
            return false;
        int cols = game.Cols;
        int rows = game.Rows;

        if (objectLocation.Row < objectSize)
            return false;
        if (objectLocation.Col < objectSize)
            return false;
        if (rows - objectLocation.Row < objectSize)
            return false;
        if (cols - objectLocation.Col < objectSize)
            return false;
        return true;
    }
    
    public static void GetDistancesOfElvesFromEnemyElves(Game game, Stack<int[]> distanceFromEnemyElves)
    {
        int[] distance = new int[(game.GetMyLivingElves().Length) * (game.GetEnemyLivingElves().Length)];
        int count = 0;
        
        foreach (Elf myElf in game.GetMyLivingElves())
        {
            foreach (Elf enemyElf in game.GetEnemyLivingElves())
            {
                distance[count] = myElf.Location.Distance(enemyElf.Location);
                count++;
            }
        }
        distanceFromEnemyElves.Push(distance);
    }
    
    //Returns -1 if location = null
    public static int NumberOfCreaturesInRange(Game game, Location location, int range, bool IsIncludeLavaGiants = false)
    {
        if (location == null) return -1;
        
        int enemyElvesInRange = 0;
        int enemyIceTrollsInRange = 0;
        int enemyLavaGiantsInRange = 0;
        
        foreach (Elf enemyElf in game.GetEnemyLivingElves())
            if (enemyElf.InRange(location, range)) enemyElvesInRange++;
            
        foreach (IceTroll enemyIceTroll in game.GetEnemyIceTrolls())
            if (enemyIceTroll.InRange(location, range)) enemyIceTrollsInRange++;
        
        if (IsIncludeLavaGiants)    
            foreach (LavaGiant enemyLavaGiant in game.GetEnemyLavaGiants())
                if (enemyLavaGiant.InRange(location, range)) enemyLavaGiantsInRange++;
            
        int totalCreaturesInRangeOfLocation = enemyElvesInRange + enemyIceTrollsInRange + enemyLavaGiantsInRange;
        
        return totalCreaturesInRangeOfLocation;
    }
    
    public static Location[] GetLocationAroundObject(Game game, Location rootObject, int rootObjectRadiusINT, int newObjectRadiusINT)
    {
        double rootObjectRadius = rootObjectRadiusINT;
        double newObjectRadius = newObjectRadiusINT;
        double angleOfRotation = 2 * (System.Math.Asin((newObjectRadius) / (rootObjectRadius + newObjectRadius))); // Alpha
        Location firstLocation = new Location((int)(rootObject.Row + rootObjectRadius + newObjectRadius), rootObject.Col);
        Location newLocation = null;
        
        Stack<Location> newCircleLocationsStack = new Stack<Location>();
        newCircleLocationsStack.Push(firstLocation);
        
        int numAngleEnterInCircle = (int)((System.Math.PI * 2) / angleOfRotation);
        
        for (int i = 0; i < numAngleEnterInCircle; i++)
        {
            newLocation = GetNextLocationInCircle(game, rootObjectRadius + newObjectRadius, angleOfRotation, rootObject);
            newCircleLocationsStack.Push(newLocation);    
            angleOfRotation += angleOfRotation;
        }
        
        Location[] newCircleLocationsArray = new Location[newCircleLocationsStack.Count];
        
        for (int i = 0; i < newCircleLocationsArray.Length; i++)
        {
            newCircleLocationsArray[i] = newCircleLocationsStack.Pop();
        }
        
        return newCircleLocationsArray;
    }
    
    public static Location GetNextLocationInCircle(Game game, double radius, double angle, Location rootObject, int object_radius = 1)
    {
        int circle_radius = radius;
        int object_radius_new = object_radius;
        int xCoordinate = (int)(rootObject.Col + (System.Math.Sin(angle) * radius));
        int yCoordinate = (int)(rootObject.Row - (System.Math.Sin(angle / 2) * object_radius_new));
        return new Location(yCoordinate, xCoordinate);
    }

    public static Location EscapeTowards(Game game, Location location, Location escapeFrom, int entityMaxSpeed = -1, Location escapeTo = null, double ratioOfEscapeTo = 2)
    {
        if (entityMaxSpeed == -1)
            entityMaxSpeed = game.ElfMaxSpeed;

        Location runTo = location;

        if (escapeFrom.Distance(location) == 0)
            return null;

        runTo = BetterTowards(game, escapeFrom, location, entityMaxSpeed + escapeFrom.Distance(location));

        if (escapeTo != null)
        {
            if (BetterTowards(game, runTo, escapeTo, entityMaxSpeed) != null)
                if (!BetterTowards(game, runTo, escapeTo, entityMaxSpeed).Equals(location))
                {
                    runTo = BetterTowards(game, runTo, escapeTo, (int)(entityMaxSpeed / ratioOfEscapeTo));
                    runTo = BetterTowards(game, location, runTo, entityMaxSpeed);
                }
        }
        if (runTo.Equals(location))
            return null;
        if (!runTo.InMap())
        {
            bool isColNotInMap = (runTo.Col > game.Cols || runTo.Col < 0);
            bool isRowNotInMap = (runTo.Row > game.Rows || runTo.Row < 0);
            if (isColNotInMap && isRowNotInMap)
                return null;
            if (isColNotInMap)
            {
                if (location.Row > escapeFrom.Row)
                    runTo = new Location(location.Row + entityMaxSpeed, location.Col);
                else
                    runTo = new Location(location.Row - entityMaxSpeed, location.Col);
            }
            if (isRowNotInMap)
            {
                if (location.Col > escapeFrom.Col)
                    runTo = new Location(location.Row, location.Col + entityMaxSpeed);
                else
                    runTo = new Location(location.Row, location.Col - entityMaxSpeed);
            }
            if (!runTo.InMap())
                return null;
        }
        return runTo;
    }
    
    public static Location GetAverageLocationFromAnArrayOfLocations(Game game, Location[] locations)
    {
        if (locations.Length == 0) return null;
        int numberOfLocations = locations.Length;
        double sumCols = 0;
        double sumRows = 0;
        
        for (int i = 0; i < numberOfLocations; i++)
        {
            if (locations[i] == null)
            {
                sumCols += 0;
                sumRows += 0;
            }
            else    
            {
                sumCols += locations[i].Col;
                sumRows += locations[i].Row;
            }
        }
        
        double finalCol = sumCols / numberOfLocations;
        double finalRow = sumRows / numberOfLocations;
        Location averageLocation = new Location((int)(finalRow), (int)(finalCol));
        
        return averageLocation;
    }
    
    public static Location PassThrough(Game game, Location passThrough, Location currentLocation, Location targetLocation, double deviationRatio = 1)
    {
        Location escapeFrom = BetterTowards(game, targetLocation, currentLocation, 200 + currentLocation.Distance(targetLocation));
        return EscapeTowards(game, currentLocation, escapeFrom, ElfFunc.GetClosestMyElfTo(game, currentLocation).MaxSpeed, passThrough, deviationRatio);
    }

    public static Location ClosestLocationToFromLocationArray(Location[] locations, Location targetLocation)
    {
        Location closestLocation = locations[0];
        for (int i = 0; i < locations.Length; i++)
        {
            if (closestLocation == null)
                closestLocation = locations[i];
            else if (locations[i] != null)
                if (locations[i].Distance(targetLocation) < closestLocation.Distance(targetLocation))
                    closestLocation = locations[i];
        }
        return closestLocation;
    }

    public static Location[] GetElementInNthPlaceForStack(Game game, int placeFromEnd, int lengthOfArray, Stack<Location[]> givenStack)
    {
        Stack<Location[]> temp = new Stack<Location[]>();
        for (int i = 0; i < placeFromEnd - 1; i++)
        {
            temp.Push(givenStack.Pop());
        }

        Location[] returnedArray = new Location[lengthOfArray];
        returnedArray = temp.Peek();

        while (temp.Count != 0)
            givenStack.Push(temp.Pop());

        return returnedArray;
    }
    
    //PLEASE INSERT ANGLE IN DEGREES!
    public static Stack<Location> GetAllLocationsAroundObject(Game game, Location location, int range, double angle)
    {
        //Converting to radians
        double angleInRadians = (double)( (double)(angle * System.Math.PI) / 180 );
        double originalAngeInRadians = angleInRadians;
        
        Stack<Location> allLocationsAroundObject = new Stack<Location>();
        
        allLocationsAroundObject.Push(new Location(location.Row, location.Col + range));
        while (angleInRadians <= System.Math.PI * 2)
        {
            double newX = location.Col + System.Math.Cos(angleInRadians) * range;
            double newY = location.Row + System.Math.Sin(angleInRadians) * range;
            
            allLocationsAroundObject.Push(new Location((int)newY, (int)newX));
            angleInRadians += originalAngeInRadians;
        }
        
        return allLocationsAroundObject;
    }
    
    //A* Pathfinding!!!
    public static Location BetterMoveTo(Game game, Elf elf, Location targetLocation)
    {
        int range = game.ElfAttackRange + elf.MaxSpeed * 2;
        int angleInDegrees = 1;
        Stack<Location> possibleLocationsAroundElf = GetAllLocationsAroundObject(game, elf.Location, range, angleInDegrees);
        Stack<Location> locationsWithNoThreat = new Stack<Location>();
        
        bool isNoElfAroundLocation = true;
        bool isNoIceTrollAroundLocation = true;
        while (possibleLocationsAroundElf.Count != 0)
        {
            foreach(Elf enemyElf in game.GetEnemyLivingElves())
            {
                if (enemyElf.InRange(possibleLocationsAroundElf.Peek(), range))
                    isNoElfAroundLocation = false;
            }
            
            foreach(IceTroll enemyIceTroll in game.GetEnemyIceTrolls())
            {
                if (enemyIceTroll.InRange(possibleLocationsAroundElf.Peek(), range))
                    isNoIceTrollAroundLocation = false;
            }
            
            if (isNoElfAroundLocation && isNoIceTrollAroundLocation)
                locationsWithNoThreat.Push(possibleLocationsAroundElf.Peek());
              
            isNoElfAroundLocation = true;
            isNoIceTrollAroundLocation = true;  
            possibleLocationsAroundElf.Pop();
        }
        
        //The location that has no threat AND the closest to the target location
        Location bestLocation = GetClosestLocationFromStack(game, locationsWithNoThreat, targetLocation);
        
        return bestLocation;
    }
    
    public static Location GetClosestLocationFromStack(Game game, Stack<Location> locations, Location targetLocation)
    {
        Location closestLocationToTargetLocation = null;
        Stack<Location> tempLocationStack = new Stack<Location>();
        while (locations.Count != 0)
        {
            if (closestLocationToTargetLocation == null)
                closestLocationToTargetLocation = locations.Peek();
            else 
            {
                if (closestLocationToTargetLocation.Distance(targetLocation) > locations.Peek().Distance(targetLocation))
                    closestLocationToTargetLocation = locations.Peek();
            }
            
            tempLocationStack.Push(locations.Pop());
        }
        
        while (tempLocationStack.Count != 0)
            locations.Push(tempLocationStack.Pop());
            
        return closestLocationToTargetLocation;
    }
}