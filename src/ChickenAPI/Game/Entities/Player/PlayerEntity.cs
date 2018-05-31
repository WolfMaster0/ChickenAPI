﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using ChickenAPI.Data.AccessLayer;
using ChickenAPI.Data.TransferObjects;
using ChickenAPI.ECS.Components;
using ChickenAPI.ECS.Entities;
using ChickenAPI.Enums.Game.Character;
using ChickenAPI.Enums.Game.Entity;
using ChickenAPI.Game.Components;
using ChickenAPI.Game.Maps;
using ChickenAPI.Game.Network;
using ChickenAPI.Packets;
using ChickenAPI.Packets.Game.Server;
using ChickenAPI.Utils;

namespace ChickenAPI.Game.Entities.Player
{
    public class PlayerEntity : EntityBase, IPlayerEntity
    {
        public PlayerEntity(ISession session, CharacterDto dto) : base(EntityType.Player)
        {
            Session = session;
            Components = new Dictionary<Type, IComponent>
            {
                { typeof(VisibilityComponent), new VisibilityComponent(this) },
                {
                    typeof(MovableComponent), new MovableComponent(this)
                    {
                        Actual = new Position<short>
                        {
                            X = dto.MapX,
                            Y = dto.MapY,
                        },
                        Destination = new Position<short>
                        {
                            X = dto.MapX,
                            Y = dto.MapY,
                        },
                    }
                },
                { typeof(BattleComponent), new BattleComponent(this) },
                { typeof(CharacterComponent), new CharacterComponent(this, dto) },
                {
                    typeof(ExperienceComponent), new ExperienceComponent(this)
                    {
                        Level = dto.Level,
                        LevelXp = dto.LevelXp,
                        JobLevel = dto.JobLevel,
                        JobLevelXp = dto.JobLevelXp,
                        HeroLevel = dto.HeroLevel,
                        HeroLevelXp = dto.HeroXp,
                    }
                },
                { typeof(FamilyComponent), new FamilyComponent(this) },
                { typeof(InventoryComponent), new InventoryComponent(this) },
                {
                    typeof(NameComponent), new NameComponent(this)
                    {
                        Name = dto.Name
                    }
                },
                { typeof(SpecialistComponent), new SpecialistComponent(this) }
            };
        }

        public ISession Session { get; }

        public override void TransferEntity(IEntityManager manager)
        {
            base.TransferEntity(manager);

            if (!(manager is IMapLayer map))
            {
                return;
            }

            SendPacket(new CInfoPacketBase(this));
            SendPacket(new CModePacketBase(this));
            // eq
            // Equipment()

            SendPacket(new LevPacket(this));
            // Stat()
            SendPacket(new AtPacketBase(this));
            SendPacket(new CondPacketBase(this));
            SendPacket(new CMapPacketBase(map.Map));
            // StatChar()
            SendPacket(new InPacketBase(this));
            // Pairy()
            // Pst()
            // mates In()
            // Act6() : Act()
            // PInitPacket
            // ScPacket
            // ScpStcPacket
            // FcPacket
            // Act4Raid ? DgPacket() : RaidMbf
            // MapDesignObjects()
            // MapDesignObjectsEffects
            // MapItems()
            // Gp()
        }

        public void SendPacket(IPacket packetBase) => Session.SendPacket(packetBase);

        public void SendPackets(IEnumerable<IPacket> packets) => Session.SendPackets(packets);

        private void Save()
        {
            try
            {
                var characterService = Container.Instance.Resolve<ICharacterService>();
                characterService.Update(new CharacterDto());
                var itemInstanceService = Container.Instance.Resolve<IItemInstanceService>();
                itemInstanceService.Update(new List<ItemInstanceDto>());
            }
            catch (Exception e)
            {
                Log.Error("[SAVE]", e);
            }
        }

        public override void Dispose()
        {
            //Save();
            GC.SuppressFinalize(this);
        }
    }
}