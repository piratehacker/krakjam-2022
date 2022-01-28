﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cyberultimate.Unity;
using Player;
using Scoreboard;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private int levelCount;
        [SerializeField] private AnimationCurve difficulty;

        public int CurrentLevel { get; private set; }

        public Transform startingPosA;
        public Transform startingPosB;
        public Elevator startingElevator;
        public Elevator finishElevator;
        public Transform player;
        public Transform cameraHolder;

        private int width, height;
        private float spaceX, spaceZ;

        private void Start()
        {
            (startingElevator, finishElevator, elevator1Z) = (finishElevator, startingElevator, elevator2Z);
            width = GenerateRoom.Current.width;
            height = GenerateRoom.Current.height;
            spaceX = GenerateRoom.Current.spaceX;
            spaceZ = GenerateRoom.Current.spaceZ;

            elevator2Z = (int) (startingElevator.transform.position.z / spaceZ)+1;

            if (startingPosA != null)
            {
                player.position = startingPosA.position;
                cameraHolder.localRotation = startingPosA.localRotation;
            }

            NextLevel();

            Scoreboard.Scoreboard.Current.NewRun();
        }

        public void NextLevel()
        {
            CurrentLevel++;

            (startingElevator, finishElevator, elevator1Z) = (finishElevator, startingElevator, elevator2Z);

            Scoreboard.Scoreboard.Current.PostLevelData();

            GenerateLevel();

            EnemySpawner.Current.transform.KillAllChildren();
            EnemySpawner.Current.StartSpawning(); 

            // startingElevator.Open();
            startingElevator.active = false;
            finishElevator.active = true;
        }

        private int elevator1Z;
        private int elevator2Z;
        private void GenerateLevel()
        {
            elevator2Z = UnityEngine.Random.Range(0, height * 2);
            Vector3 newElevatorPos = new Vector3(finishElevator.transform.position.x, finishElevator.transform.position.y,elevator2Z*spaceZ);

            finishElevator.transform.position = newElevatorPos;

            GenerateRoom.Current.transform.KillAllChildren();
            GenerateRoom.Current.Generate(elevator1Z, elevator2Z);
            ObjectGeneration.Current.GenerateObjects();
            Debug.Log("done");
        }
    }
}
