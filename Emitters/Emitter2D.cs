﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ParticleSystem.Helpers;
using ParticleSystem.Providers;
using ParticleSystem.Interpolators;

namespace ParticleSystem.Emitters
{
    public abstract class Emitter2D : IEmitter
    {
        protected Random _rnd;

        protected Emitter2D(
            ValueProvider<Color, ColorInterpolator> colorProvider,
            ValueProvider<float, FloatInterpolator> scaleProvider,
            ValueProvider<float, FloatInterpolator> opacityProvider)
        {
            this.ColorProvider = colorProvider;
            this.ScaleProvider = scaleProvider;
            this.OpacityProvider = opacityProvider;

            Particles = new List<Particle>();
            _rnd = new Random();
        }

        // How many particles are emitted per frame update (1/60s)
        public int EmissionRate { get; set; } = 1;
        public List<Particle> Particles { get; }
        public Vector2 Location { get; set; }
        public int Lifetime { get; set; } = 600;
        public int ParticleMaximumLife { get; set; } = 180;
        public int StartDelay { get; set; } = 0;
        public double ParticleMaxSpeed { get; set; } = 2;
        public int Age { get; set; }
        public bool Loop { get; set; } = false;
        public bool Active { get; set; } = false;
        public bool Prewarm { get; set; } = false;
        public Texture2D Texture { get; set; }
        public ValueProvider<Color, ColorInterpolator> ColorProvider { get; set; }
        public ValueProvider<float, FloatInterpolator> ScaleProvider { get; set; }
        public ValueProvider<float, FloatInterpolator> OpacityProvider { get; set; }

        public void Trigger()
        {
            Active = true;

            if (Loop)
            {
                Particles.Clear();
            }

            if (Loop && Prewarm)
            {
                ResetToStart();

                for (int i = 0; i < Lifetime; i++)
                {
                    Update();
                }
            }

            ResetToStart();
        }

        public void Stop()
        {
            Active = false;
        }

        public virtual void Update()
        {
            if (Age == Lifetime)
            {
                if (!Loop)
                    Active = false;

                else
                    ResetToStart();
            }

            else
            {
                Age++;
            }

            if (Age > 0 && Active)
            {
                EmitParticles();
            }

            UpdateParticles();
        }

        public void ResetToStart()
        {
            if (!Prewarm)
                Age = 0 - StartDelay;

            else
                Age = 0;
        }

        public void EmitParticles()
        {
            for (int i = 0; i < EmissionRate; i++)
            {
                Particles.Add(CreateNewParticle());
            }
        }

        public virtual void UpdateParticles()
        {
            foreach (var particle in Particles.ToList())
            {
                // If the particle is alive, update it
                if (particle.Age < ParticleMaximumLife)
                {
                    UpdateParticle(particle);
                }

                // The particle is dead, remove it
                else
                {
                    Particles.Remove(particle);
                }
            }
        }

        public virtual void Render(SpriteBatch spriteBatch)
        {
            foreach (var particle in Particles)
            {
                RenderParticle(particle, spriteBatch);
            }
        }

        public virtual void UpdateParticle(Particle particle)
        {
            particle.Age++;
            var particlePercent = (float)particle.Age / ParticleMaximumLife;
            particle.PreviousPosition = particle.Position;
            particle.Position += particle.Velocity;
            //particle.Angle = Vector2Helper.GetAngleInRadians(particle.Velocity);
            particle.Angle = Vector2Helper.GetAngleInDegrees(particle.Velocity);
            particle.Scale = ScaleProvider.GetValue(particlePercent);
            particle.Color = ColorProvider.GetValue(particlePercent);
            particle.Opacity = OpacityProvider.GetValue(particlePercent);
        }

        public virtual void RenderParticle(Particle particle, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, particle.Position, Texture.Bounds,
                particle.Color * particle.Opacity, particle.Angle, new Vector2(Texture.Width / 2, Texture.Height / 2), particle.Scale,
                SpriteEffects.None, 0);
        }

        public virtual Particle CreateNewParticle()
        {
            var emitterPercent = (float)Age / Lifetime;

            Particle particle = new Particle();
            particle.Velocity = GetRandomVelocity();
            particle.Position = InitializePosition();
            //particle.Color = ColorProvider.GetValue(emitterPercent);
            return particle;
        }

        public virtual Vector2 InitializePosition()
        {
            return new Vector2(
                Location.X - Texture.Width * ScaleProvider.GetValue(0) * 0.5f,
                Location.Y - Texture.Height * ScaleProvider.GetValue(0) * 0.5f);
        }

        private Vector2 GetRandomVelocity()
        {
            var r = ParticleMaxSpeed * Math.Sqrt(_rnd.NextDouble());
            var theta = _rnd.NextDouble() * 2 * Math.PI;

            var x = r * Math.Cos(theta);
            var y = r * Math.Sin(theta);

            return new Vector2((float)x, (float)y);
        }
    }
}
