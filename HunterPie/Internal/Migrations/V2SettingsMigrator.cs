﻿using HunterPie.Core.Client.Configuration;
using HunterPie.Core.Domain.Interfaces;
using System;

namespace HunterPie.Internal.Migrations
{
    internal class V2SettingsMigrator : ISettingsMigrator
    {
        public bool CanMigrate(IVersionedConfig oldSettings)
        {
            return oldSettings.GetType() == typeof(Config);
        }

        public IVersionedConfig Migrate(IVersionedConfig oldSettings)
        {
            if (oldSettings is Config config)
            {
                V2Config v2Config = new V2Config()
                {
                    Client = config.Client,
                    Rise = new()
                    {
                        RichPresence = config.RichPresence,
                        Overlay = config.Overlay,
                    },
                    Development = config.Debug
                };

                return v2Config;
            }

            throw new InvalidCastException($"old config must be of type {typeof(Config)}, but was {oldSettings.GetType()}");
        }

        public Type GetRequiredType()
        {
            return typeof(Config);
        }
    }
}
