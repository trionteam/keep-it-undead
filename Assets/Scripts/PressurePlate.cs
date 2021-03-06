﻿using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    private Sprite _pressedSprite = null;

    [SerializeField]
    private Sprite _depressedSprite = null;

    public bool IsPressed
    {
        get; private set;
    }

    private Collider2D _plateCollider;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _plateCollider = GetComponent<Collider2D>();
        Debug.Assert(_plateCollider != null);

        _renderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_renderer != null);

        Debug.Assert(_pressedSprite != null);
        Debug.Assert(_depressedSprite != null);
    }

    private void Start()
    {
        IsPressed = false;
    }

    void Update()
    {
        var candidates = new Collider2D[1];
        var filter = new ContactFilter2D();
        // We're looking for contacts only in the same collision layer as the pressure
        // plate (which is by default the ground collisions layer). Any colliders in this
        // layer are assumed to be moving on the ground, and should thus be able to
        // press the plate.
        filter.SetLayerMask((1 << Layers.GroundMovementPartial) |
                            (1 << Layers.GroundMovementSolid) |
                            (1 << Layers.GroundMovementUnlimited));
        var numCandidates = _plateCollider.OverlapCollider(filter, candidates);
        IsPressed = numCandidates > 0;
        _renderer.sprite = IsPressed ? _pressedSprite : _depressedSprite;
    }
}
