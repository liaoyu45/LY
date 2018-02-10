using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace LivingDB {
	/// <summary>
	/// 多表存储的默认规则（sql server）。
	/// </summary>
	public class DefaultDbLoader : IDbLoader {
		private static PluralizationService s = PluralizationService.CreateService(new CultureInfo(0x0409));
		public string Prepare => @"
if 0 < (select count(*) from sysobjects where xtype='U' and name='__MigrationHistory')
	drop table __MigrationHistory";

		public string ProviderName => "System.Data.SqlClient";

		public virtual string CreateTable(Type t) {
			return t.GetCustomAttribute<SqlAttribute>(true).Sql;
		}

		public virtual string GetDynamicTableName(object m) {
			var t = m.GetType();
			var v = (int)t.GetProperty("Id").GetValue(m) / Size(t);
			var n = GetTableName(t);
			return v > 0 ? n + v : n;
		}

		public virtual string[] GetRecord(params string[] tables) {
			return new[] {
				string.Format(GetRecordSqlFormat, tables.Aggregate(string.Empty, (s, ss) => s + ss + ',').Trim(','))
			};
		}

		public virtual int Size(Type m) {
			return m.GetCustomAttribute<SqlAttribute>().Size;
		}

		public virtual string GetTableName(Type m) {
			return s.Pluralize(m.Name);
		}

		public string CreateTable(string sqlFormat, object model) {
			return string.Format(sqlFormat, GetDynamicTableName(model));
		}


		#region GetRecordSqlFormat
		private const string GetRecordSqlFormat = @"
declare
	@input nvarchar(max)='{0}';
declare @ts table (name nvarchar(max));--table names
declare
	@Index int=charindex(',',@input),
	@tn nvarchar(max)
while (@Index>0)
begin
    set @tn=ltrim(rtrim(substring(@input, 1, @Index-1)))
    if @tn<>''
        insert into @ts Values(@tn)
    set @input = substring(@input, @Index+1, len(@input))
    set @Index = charindex(',', @input)
end    
set @tn=ltrim(rtrim(@input))
if @tn<>''
    insert into @ts Values(@tn)

declare @dt table(name nvarchar(max));--detail tables
declare
	@mc int,--main tables'count
	@mi int=0,--main table interator
	@s nvarchar(max)='',--sql
	@dn nvarchar(max),--detail table name
	@dc int;
select @mc=count(*)from @ts;
while @mi<@mc
begin
	declare @di int=0;
	select @tn=name from @ts order by name offset(@mi) rows fetch next 1 rows only
	insert @dt select name from sysobjects where xtype='U' and substring(name,1,len(@tn))=@tn and len(name)>len(@tn) order by name
	insert @dt values(@tn);
	select @dc=count(*) from @dt
	while @di<@dc
	begin
		select @dn=name from @dt order by name offset (@di)rows fetch next 1 rows only
		set @s=@s+'select '''+@tn+'''""Main"",'''+@dn+''' ""Detail"",count(*)""Count"" from '+@dn+' union all '	
		set @di = @di + 1
	end
	delete @dt;
			set @mi = @mi + 1
end
if len(@s)>0
begin
	set @s = substring(@s, 1, len(@s) - 10)
	exec(@s)
end";
		#endregion
	}
}
