using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.Replication;

namespace PromptEngineering.Utils.SummmaryGenerators
{
    public interface ISummaryModel : IEquatable<ISummaryModel>
    {
        
    }
}