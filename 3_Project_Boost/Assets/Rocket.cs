using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Rocket : MonoBehaviour
{
    //public Text fuelText;
    [SerializeField] float rcsThrust = 120f;
    [SerializeField] float mainThrust = 2000f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float fuel = 100f;
    [SerializeField] float fuelConsumptionRate = 0.1f;

    //IMPORTANT ADD TO DOCS!
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip successSound;

    //IMPORTANT ADD TO DOCS!
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;
    public FuelBar fuelBar;

    enum State { Alive, Dying, Transcending, Debug }
    State state = State.Alive;

    bool collisionDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        fuelBar.SetMaxFuel(fuel);
        rigidBody = GetComponent<Rigidbody>(); //this gives us access to the rigidbody in unity
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            Rotate();
            EmptyFuel();
        }

        if (UnityEngine.Debug.isDebugBuild)
        {
            SkipLevel();
            DisableCollision();
        }

    }

    void OnGUI()
    {
        GUI.Label(new Rect(230, 0, 400, 400), "Controls: A - Rotate Left | D - Rotate Right | Spacebar - Boost");
    }

    void OnCollisionEnter(Collision collision)
    {
        //if not alive IMMEDIATELY stop method. This fixes the multiple collision during a state change
        if (state != State.Alive || collisionDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly": //tag name from inspector
                //if friendly do nothing
                break;
            case "Fuel":
                break;
            case "Finish":
                SuccessSequence();
                Invoke("LoadNextScene", levelLoadDelay); //parametrise time, invokes a method at specified time
                break;
            default:
                DeathSequence();
                //Invoke("LoadNextScene", levelLoadDelay);
                break;
        }
    }

    private void LoadNextScene()//allow for more than 2 levels
    {

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int noOfLevels = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = currentScene + 1;

        if (nextSceneIndex == noOfLevels)
        {
            nextSceneIndex = 0;
        }

        SceneManager.LoadScene(nextSceneIndex);
        
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    // Invoked at deathSequence() to restart level when death occurs
    private void RestartLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    void EmptyFuel()
    {
        if (fuel < 1)
        {
            fuel = 0; //set fuel to 0 to avoid becoming negative value (will affect GUI as well)
            mainThrust = 0f;
            mainEngineParticles.Stop();
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
            UseFuel(fuelConsumptionRate);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        fuel -= fuelConsumptionRate;
        fuelBar.SetFuel(fuel);
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); //.up applies to the y-axis. 
        if (!audioSource.isPlaying) //so audio doesn't layer (play multiple times at the same time)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
        
    }

    private void UseFuel(float fuelConsumptionRate)
    {
        fuel -= fuelConsumptionRate;
        fuelBar.SetFuel(fuel);
    }

    private void SuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(successSound);
        successParticles.Play();
    }

    private void DeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("RestartLevel", levelLoadDelay);
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;// take manual control of rotation

        //float rcsThrust = 120f;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame); //rotate left
        }        
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame); // rotate right
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }

    private void SkipLevel()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            state = State.Transcending;
            LoadNextScene();
        }
        
    }

    private void DisableCollision()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;
        }
    }

}
