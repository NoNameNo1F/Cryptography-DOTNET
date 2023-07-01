// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;

// namespace Cryptography.models {
//     public class CryptoContext : DbContext<File> {

//         public CryptoContext(DbContextOptions<CryptoContext> options) : base(options){
//             //.......
//             // this.Roles
//         }

//         protected override void OnConfiguring(DbContextOptionsBuilder builder)
//         {
//             base.OnConfiguring(builder);
//         }
//         // protected override void OnModelCreating(ModelBuilder modelBuilder){
//         //     base.OnModelCreating(modelBuilder);

//         //     foreach ( var entityType in modelBuilder.Model.GetEntityTypes()){
//         //         var tableName = entityType.GetTableName();
//         //         if(tableName.StartsWith("AspNet")){
//         //             entityType.SetTableName(tableName.Substring(6));
//         //         }
//         //     }
//         // }
//     }
// }