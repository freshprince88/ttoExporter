﻿//-----------------------------------------------------------------------
// <copyright file="IMatchTransitionStatistics.cs" company="Fakultät für Sport- und Gesundheitswissenschaft">
//    Copyright © 2013, 2014 Fakultät für Sport- und Gesundheitswissenschaft
// </copyright>
//-----------------------------------------------------------------------

namespace ttoExporter.Statistics
{
    /// <summary>
    /// Statistics based on match transitions.
    /// </summary>
    public interface IMatchTransitionStatistics : IMatchStatistics
    {
        /// <summary>
        /// Gets the transitions backing this statistics.
        /// </summary>
        Transitions Transitions { get; }
    }
}
