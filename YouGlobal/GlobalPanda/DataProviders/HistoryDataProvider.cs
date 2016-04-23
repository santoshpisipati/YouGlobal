using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Data;

/// <summary>
/// Summary description for HistoryDataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class HistoryDataProvider
    {
        public int insertHistory(HistoryInfo info)
        {
            int historyId = 0;

            string sql = "insert into history (userId,moduleId,typeId,recordId,modifiedDate,parentid) values (?userId,?moduleId,?typeId,?recordId,?modifiedDate,?parentid);select last_insert_id()";
            MySqlParameter[] param ={
                                        new MySqlParameter("userId",info.UserId),
                                        new MySqlParameter("moduleId",info.ModuleId),
                                        new MySqlParameter("typeId",info.TypeId),
                                        new MySqlParameter("recordId",info.RecordId),
                                        new MySqlParameter("modifiedDate",DateTime.UtcNow),
                                        new MySqlParameter("parentid",info.ParentRecordId)
                                    };
            historyId = Convert.ToInt32(DAO.ExecuteScalar(sql, param));

            foreach (HistoryDetailInfo detail in info.Details)
            {
                sql = "insert into historyDetail (historyId,columnName,oldValue,newValue) values (?historyId,?columnName,?oldValue,?newValue)";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("historyId", historyId), new MySqlParameter("columnName", detail.ColumnName), new MySqlParameter("oldValue", detail.OldValue), new MySqlParameter("newValue", detail.NewValue));
            }

            return historyId;
        }

        public DataTable getHistory(string keyword, int userid, DateTime? from, DateTime? to, int moduleid, string sort)
        {
            string sql = "select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDateFormat, h.modifieddate,h.parentid " +
                " from history h inner join historydetail d on h.historyid=d.historyid left join users u on u.userid=h.userid " +
                " where (oldvalue like concat_ws(?keyword,'%','%') or newvalue like concat_ws(?keyword,'%','%')) and (h.userid=?userid or ?userid=0) and (h.moduleid=?moduleid or ?moduleid=0) " +
                " and (h.modifiedDate>=?from or ?from is null) and (h.modifiedDate<=?to or ?to is null) order by " + sort;
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("userid", userid), new MySqlParameter("moduleid", moduleid), new MySqlParameter("from", from), new MySqlParameter("to", to));
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            string[] tbl = new string[1];
            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            dt = ds.Tables[0];
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable getCandidateEmailHistory(int moduleId, int candidateId)
        {
            string sql = "select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid left join users u on u.userid=h.userid " +
                " inner join candidates_emails ce on ce.emailid=h.recordid inner join candidates c on c.candidateid=ce.candidateid " +
                " where c.candidateid=?candidateid and h.moduleid=?moduleid union " +
                " select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid inner join users u on u.userid=h.userid " +
                "  inner join candidates c on c.candidateid=h.parentid " +
                " where c.candidateid=?candidateid and h.moduleid=?moduleid order by dt desc";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("moduleid", moduleId));
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            string[] tbl = new string[1];
            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            dt = ds.Tables[0];
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable getCandidateWorkHistory(int moduleId, int candidateId)
        {
            string sql = "select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid left join users u on u.userid=h.userid " +
                " inner join candidates_workhistories ce on ce.candidateworkhistoryid=h.recordid inner join candidates c on c.candidateid=ce.candidateid " +
                " where c.candidateid=?candidateid and h.moduleid=?moduleid union " +
                " select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid inner join users u on u.userid=h.userid " +
                "  inner join candidates c on c.candidateid=h.parentid " +
                " where c.candidateid=?candidateid and h.moduleid=?moduleid order by dt desc";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("moduleid", moduleId));
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            string[] tbl = new string[1];
            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            dt = ds.Tables[0];
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public MySqlDataReader getCandidateWorkHistoryById(int moduleId, int recordId)
        {
            string sql = "select h.recordid,u.username,h.modifieddate,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDateFormat,hd.columnname " +
                         " from history h join historydetail hd on hd.historyid=h.historyid join users u on h.userid=u.userid where h.recordid=?recordid and h.moduleid=?moduleid order by modifieddate";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("recordId", recordId), new MySqlParameter("moduleId", moduleId));
            return dr;
        }

        public DataTable getCandidateJobalertHistory(int moduleId, int candidateId)
        {
            string sql = "select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid left join users u on u.userid=h.userid " +
                " inner join job_alert ja on ja.job_alertid=h.recordid  " +
                " where ja.candidateid=?candidateid and (h.moduleid =?moduleid or h.moduleid=?moduleid1) union " +
                " select moduleId,typeId,recordId,columnName,COALESCE(oldValue,'') as oldValue,newValue,username,Date_Format(h.modifieddate,'%d-%b-%Y-%T') as modifiedDate,h.modifieddate as dt  " +
                " from history h inner join historydetail d on h.historyid=d.historyid left join users u on u.userid=h.userid " +
                "  inner join candidates c on c.candidateid=h.parentid " +
                " where c.candidateid=?candidateid and (h.moduleid =?moduleid or h.moduleid=?moduleid1) order by dt desc";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("moduleid", moduleId), new MySqlParameter("moduleid1", (int)HistoryInfo.Module.jobalert_invite));
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            string[] tbl = new string[1];
            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            dt = ds.Tables[0];
            reader.Close();
            reader.Dispose();
            return dt;
        }
    }
}