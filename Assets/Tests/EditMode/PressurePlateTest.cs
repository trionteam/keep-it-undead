﻿using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestPressurePlate
    {
        private GameObject _cloudPrefab;
        private GameObject _cowPrefab;
        private GameObject _pressurePlatePrefab;
        private GameObject _zombiePrefab;

        [UnitySetUp]
        public IEnumerator SetUpTest()
        {
            _cloudPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs.Sprays.Cloud);
            Assert.IsNotNull(_cloudPrefab);
            _cowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs.Characters.Cow);
            Assert.IsNotNull(_cowPrefab);
            _pressurePlatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs.Triggers.PressurePlate);
            Assert.IsNotNull(_pressurePlatePrefab);
            _zombiePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs.Characters.Zombie);
            Assert.IsNotNull(_zombiePrefab);

            yield return new EnterPlayMode();
            EditorSceneManager.LoadSceneInPlayMode("Tests/EditMode/UnitTestScene", new LoadSceneParameters(LoadSceneMode.Single));
            Time.timeScale = 10.0f;
        }

        [UnityTearDown]
        public IEnumerator TearDownTest()
        {
            Time.timeScale = 1.0f;
            yield return new ExitPlayMode();
        }

        /// <summary>
        /// Tests the pressure plate with the given prefab. Instantiates a pressure plate at
        /// Vector3.zero, and the given prefab at a given position (Vector3.zero by default),
        /// and checks whether the pressure plate is considered to be pressed.
        /// </summary>
        /// <param name="prefab">The prefab used to test the trigger.</param>
        /// <param name="isExpectedToTrigger"><c>true</c> if the prefab at the given position
        /// is expected to trigger the pressure plate.</param>
        /// <param name="prefabPosition">The position of the prefab. Vector3.zero by default.
        /// </param>
        /// <returns>A Unity coroutine that runs the test.</returns>
        private IEnumerator TestWithPrefab(GameObject prefab,
                                           bool isExpectedToTrigger,
                                           Vector3 prefabPosition = new Vector3())
        {
            // Instantiate a pressure plate.
            var pressurePlate = GameObject.Instantiate(_pressurePlatePrefab).GetComponent<PressurePlate>();
            Assert.IsNotNull(pressurePlate);
            // The pressure plate should be unpressed (there is nothing to press it).
            Assert.IsFalse(pressurePlate.IsPressed);

            // Instantiate a triggering object at more or less the same position.
            var triggeringObject = GameObject.Instantiate(prefab, prefabPosition, Quaternion.identity);
            Assert.IsNotNull(triggeringObject);
            yield return null;

            // In the next frames, the pressure plate should be pressed.
            for (int i = 0; i < 10; ++i)
            {
                Assert.AreEqual(isExpectedToTrigger, pressurePlate.IsPressed);
                yield return null;
            }

            // Move the triggering object far far away.
            triggeringObject.transform.position = new Vector3(100.0f, 0.0f, 0.0f);
            yield return null;

            // In the next frame, the pressure plate should be depressed.
            Assert.IsFalse(pressurePlate.IsPressed);

            GameObject.Destroy(pressurePlate);
            GameObject.Destroy(triggeringObject);
        }

        /// <summary>
        /// Tests that a cow standing over the pressure plate triggers it.
        /// </summary>
        [UnityTest]
        public IEnumerator RespondsToCows()
        {
            return TestWithPrefab(_cowPrefab, true);
        }

        /// <summary>
        /// Tests that a zombie standing over the pressure plate triggers it.
        /// </summary>
        [UnityTest]
        public IEnumerator RespondsToZombies()
        {
            return TestWithPrefab(_zombiePrefab, true);
        }

        /// <summary>
        /// Tests that a zombie standing next to the pressure plate (with other
        /// colliders overlapping it) does not trigger the plate.
        /// </summary>
        [UnityTest]
        public IEnumerator DoesNotRespondToZombieDetection()
        {
            return TestWithPrefab(_zombiePrefab, false, new Vector3(0.0f, -0.265f, 0.0f));
        }

        /// <summary>
        /// Tests that a cloud flying over the plate does not trigger it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator DoesNotRespondToCloud()
        {
            return TestWithPrefab(_cloudPrefab, false);
        }
    }
}
