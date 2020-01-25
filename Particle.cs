﻿using Microsoft.Xna.Framework;

namespace ParticleSystem
{
    public class Particle
    {
        public float Opacity { get; set; } = 1f;
        public float Size { get; set; } = 10f;
        public int MaximumLife { get; set; } = 120;
        public int Age { get; set; }
        public Color Color { get; set; } = Color.White;        
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;
    }
}