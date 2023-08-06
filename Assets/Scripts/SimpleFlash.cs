using System.Collections;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _duration;
    private SpriteRenderer _spriteRenderer;
    private Material _originalMaterial;

    private Coroutine _flashRoutine;

    private WaitForSeconds _waitForSeconds;

    void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_duration);
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _originalMaterial = _spriteRenderer.material;
    }

    public void Flash()
    {
        // If the flashRoutine is not null, then it is currently running.
        if (_flashRoutine != null)
        {
            // In this case, we should stop it first.
            // Multiple FlashRoutines the same time would cause bugs.
            StopCoroutine(_flashRoutine);
        }

        // Start the Coroutine, and store the reference for it.
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // Swap to the flashMaterial.
        _spriteRenderer.material = _flashMaterial;

        // Pause the execution of this function for "duration" seconds.
        yield return _waitForSeconds;

        // After the pause, swap back to the original material.
        _spriteRenderer.material = _originalMaterial;

        // Set the routine to null, signaling that it's finished.
        _flashRoutine = null;
    }

    public void FlashForSeconds(float seconds)
    {
        int flashCount = (int)(seconds / _duration);
        StartCoroutine(FlashForSecondsRoutine(flashCount));
    }

    private IEnumerator FlashForSecondsRoutine(int flashCount)
    {
        float flashRate = 1.0f;
        print("flashCount" + flashCount);

        WaitForSeconds waitForSeconds = new WaitForSeconds(flashRate);

        while (flashCount > 0)
        {
            Flash();
            yield return new WaitForSeconds(flashRate);
            flashCount--;
        }
    }
}
