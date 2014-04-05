
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace kabuto
{
	public class EnemySpawner : GameEntity
    {
    	public float SpawnCounter;
        public float SpawnRate;
        public int Type;
        public int Total;

        public override void Tick(float dt)
        {
        	base.Tick(dt);

            SpawnCounter += dt;

            if (SpawnCounter > SpawnRate)
            {
                SpawnCounter -= SpawnRate;
                SpawnEnemy();
            }
        }

        public void SpawnEnemy()
        {
			// don't spawn any more if player is dead
			if (Game.Instance.PlayerDead)
				return;

        	// -1 is infinite spawning
        	if (Total == 0)
				return;
			
			// DEBUG
			//Type = 2;
			//return;

			Logger.Debug("Spawn Enemy");
        	switch (Type)
			{
			case 0:
				Game.Instance.AddQueue.Add(new EnemyTurtle() { Position = this.Position, });
				break;
			default:
				Common.Assert(false, "default break");
				break;
			}
			
			Total -= 1;
        }
    }
}
