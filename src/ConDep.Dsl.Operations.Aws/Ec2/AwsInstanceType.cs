namespace ConDep.Dsl
{
    public struct AwsInstanceType
    {
        private static AwsT2InstanceType _t2 = new AwsT2InstanceType();
        private static AwsM3InstanceType _m3 = new AwsM3InstanceType();
        private static AwsC4InstanceType _c4 = new AwsC4InstanceType();
        private static AwsC3InstanceType _c3 = new AwsC3InstanceType();
        private static AwsR3InstanceType _r3 = new AwsR3InstanceType();
        private static AwsG2InstanceType _g2 = new AwsG2InstanceType();
        private static AwsI2InstanceType _i2 = new AwsI2InstanceType();
        private static AwsHS1InstanceType _hs1 = new AwsHS1InstanceType();
        private static AwsM1InstanceType _m1 = new AwsM1InstanceType();
        private static AwsC1InstanceType _c1 = new AwsC1InstanceType();
        private static AwsCC2InstanceType _cc2 = new AwsCC2InstanceType();
        private static AwsCG1InstanceType _cg1 = new AwsCG1InstanceType();
        private static AwsM2InstanceType _m2 = new AwsM2InstanceType();
        private static AwsCR1InstanceType _cr1 = new AwsCR1InstanceType();
        private static AwsHI1InstanceType _hi1 = new AwsHI1InstanceType();
        private static AwsT1InstanceType _t1 = new AwsT1InstanceType();

        /// <summary>
        /// General Purpose. 
        /// Use Cases: Development environments, build servers, code repositories, 
        /// low-traffic web applications, early product experiments, small databases.  
        /// </summary>
        public static AwsT2InstanceType T2 { get { return _t2; } }

        /// <summary>
        /// General Purpose. 
        /// Use Cases: Small and mid-size databases, data processing tasks that 
        /// require additional memory, caching fleets, and for running backend 
        /// servers for SAP, Microsoft SharePoint, and other enterprise applications.
        /// </summary>
        public static AwsM3InstanceType M3 { get { return _m3; } }

        /// <summary>
        /// Compute Optimized.
        /// </summary>
        public static AwsC4InstanceType C4 { get { return _c4; } }

        /// <summary>
        /// Compute Optimized.
        /// Use Cases: High performance front-end fleets, web-servers, batch processing, 
        /// distributed analytics, high performance science and engineering applications, 
        /// ad serving, MMO gaming, video-encoding, and distributed analytics.
        /// </summary>
        public static AwsC3InstanceType C3 { get { return _c3; } }

        /// <summary>
        /// Memory Optimized.
        /// Use Cases: We recommend memory-optimized instances for high performance databases, 
        /// distributed memory caches, in-memory analytics, genome assembly and analysis, 
        /// larger deployments of SAP, Microsoft SharePoint, and other enterprise applications.
        /// </summary>
        public static AwsR3InstanceType R3 { get { return _r3; } }

        /// <summary>
        /// GPU Optimized.
        /// Use Cases: Game streaming, video encoding, 3D application streaming, and other 
        /// server-side graphics workloads.
        /// </summary>
        public static AwsG2InstanceType G2 { get { return _g2; } }

        /// <summary>
        /// Storage Optimized.
        /// Use Cases: NoSQL databases like Cassandra and MongoDB, scale out transactional 
        /// databases, data warehousing, Hadoop, and cluster file systems.
        /// </summary>
        public static AwsI2InstanceType I2 { get { return _i2; } }

        /// <summary>
        /// Storage Optimized.
        /// Use Cases: Data warehousing, Hadoop/MapReduce, Parallel file systems
        /// </summary>
        public static AwsHS1InstanceType HS1 { get { return _hs1; } }

        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsM1InstanceType M1 { get { return _m1; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsC1InstanceType C1 { get { return _c1; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsCC2InstanceType CC2 { get { return _cc2; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsCG1InstanceType CG1 { get { return _cg1; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsM2InstanceType M2 { get { return _m2; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsCR1InstanceType CR1 { get { return _cr1; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsHI1InstanceType HI1 { get { return _hi1; } }
        /// <summary>
        /// Warning: Previous Generation
        /// </summary>
        public static AwsT1InstanceType T1 { get { return _t1; } }
    }

    public struct AwsT2InstanceType
    {
        /// <summary>
        /// vCPU: 1, CPU Credits/h: 6, Mem: 1GiB, Storage: EBS-Only
        /// </summary>
        public string Micro { get { return "t2.micro"; } }

        /// <summary>
        /// vCPU: 1, CPU Credits/h: 12, Mem: 2GiB, Storage: EBS-Only
        /// </summary>
        public string Small { get { return "t2.small"; } }

        /// <summary>
        /// vCPU: 2, CPU Credits/h: 24, Mem: 4, Storage: EBS-Only
        /// </summary>
        public string Medium { get { return "t2.medium"; } }
    }

    public class AwsM3InstanceType
    {
        /// <summary>
        /// vCPU: 1, Mem: 3.75GiB, SSD Storage (GB): 1 x 4
        /// </summary>
        public string Medium { get { return "m3.medium"; } }

        /// <summary>
        /// vCPU: 2, Mem: 7.5GiB, SSD Storage (GB): 1 x 32
        /// </summary>
        public string Large { get { return "m3.large"; } }

        /// <summary>
        /// vCPU: 4, Mem: 15GiB, SSD Storage (GB): 2 x 40
        /// </summary>
        public string XLarge { get { return "m3.xlarge"; } }

        /// <summary>
        /// vCPU: 8, Mem: 30GiB, SSD Storage (GB): 2 x 80
        /// </summary>
        public string X2Large { get { return "m3.2xlarge"; } }
    }

    public struct AwsC4InstanceType
    {
        /// <summary>
        /// vCPU: 2, Mem: 3.75GiB, Storage: EBS-Only, Ded. EBS Throughput (Mbps): 500
        /// </summary>
        public string Large { get { return "c4.large"; } }

        /// <summary>
        /// vCPU: 4, Mem: 7.5GiB, Storage: EBS-Only, Ded. EBS Throughput (Mbps): 750
        /// </summary>
        public string XLarge { get { return "c4.xlarge"; } }

        /// <summary>
        /// vCPU: 8, Mem: 155GiB, Storage: EBS-Only, Ded. EBS Throughput (Mbps): 1000
        /// </summary>
        public string X2Large { get { return "c4.2xlarge"; } }

        /// <summary>
        /// vCPU: 16, Mem: 30GiB, Storage: EBS-Only, Ded. EBS Throughput (Mbps): 2000
        /// </summary>
        public string X4Large { get { return "c4.4xlarge"; } }
        
        /// <summary>
        /// vCPU: 36, Mem: 60GiB, Storage: EBS-Only, Ded. EBS Throughput (Mbps): 4000
        /// </summary>
        public string X8Large { get { return "c4.8xlarge"; } }
    }

    public struct AwsC3InstanceType
    {
        /// <summary>
        /// vCPU: 2, Mem: 3.75GiB, SSD Storage (GB): 2 x 16
        /// </summary>
        public string Large { get { return "c3.large"; } }

        /// <summary>
        /// vCPU: 4, Mem: 7.5GiB, SSD Storage (GB): 2 x 40
        /// </summary>
        public string XLarge { get { return "c3.xlarge"; } }

        /// <summary>
        /// vCPU: 8, Mem: 15GiB, SSD Storage (GB): 2 x 80
        /// </summary>
        public string X2Large { get { return "c3.2xlarge"; } }

        /// <summary>
        /// vCPU: 16, Mem: 30GiB, SSD Storage (GB): 2 x 160
        /// </summary>
        public string X4Large { get { return "c3.4xlarge"; } }

        /// <summary>
        /// vCPU: 32, Mem: 60GiB, SSD Storage (GB): 2 x 320
        /// </summary>
        public string X8Large { get { return "c3.8xlarge"; } }
    }

    public struct AwsR3InstanceType
    {
        /// <summary>
        /// vCPU: 2, Mem: 15.25GiB, SSD Storage (GB): 1 x 32
        /// </summary>
        public string Large { get { return "r3.large"; } }

        /// <summary>
        /// vCPU: 4, Mem: 30.5GiB, SSD Storage (GB): 1 x 80
        /// </summary>
        public string XLarge { get { return "r3.xlarge"; } }

        /// <summary>
        /// vCPU: 8, Mem: 61GiB, SSD Storage (GB): 1 x 160
        /// </summary>
        public string X2Large { get { return "r3.2xlarge"; } }

        /// <summary>
        /// vCPU: 16, Mem: 122GiB, SSD Storage (GB): 1 x 320
        /// </summary>
        public string X4Large { get { return "r3.4xlarge"; } }

        /// <summary>
        /// vCPU: 32, Mem: 244GiB, SSD Storage (GB): 2 x 320
        /// </summary>
        public string X8Large { get { return "r3.8xlarge"; } }
    }

    public struct AwsG2InstanceType
    {
        /// <summary>
        /// vCPU: 8, Mem: 15GiB, SSD Storage (GB): 1 x 60
        /// </summary>
        public string X2Large { get { return "g2.2xlarge"; } }
    }

    public struct AwsI2InstanceType
    {
        /// <summary>
        /// vCPU: 4, Mem: 30.5GiB, SSD Storage (GB): 1 x 800
        /// </summary>
        public string XLarge { get { return "i2.xlarge"; } }

        /// <summary>
        /// vCPU: 8, Mem: 61GiB, SSD Storage (GB): 2 x 800
        /// </summary>
        public string X2Large { get { return "i2.2xlarge"; } }

        /// <summary>
        /// vCPU: 16, Mem: 122GiB, SSD Storage (GB): 4 x 800
        /// </summary>
        public string X4Large { get { return "i2.4xlarge"; } }

        /// <summary>
        /// vCPU: 32, Mem: 244GiB, SSD Storage (GB): 8 x 800
        /// </summary>
        public string X8Large { get { return "i2.8xlarge"; } }
    }

    /// <summary>
    /// Use Cases: Data warehousing, Hadoop/MapReduce, Parallel file systems
    /// </summary>
    public struct AwsHS1InstanceType
    {
        /// <summary>
        /// vCPU: 16, Mem: 117GiB, Storage (GB): 24 x 2000
        /// </summary>
        public string X8Large { get { return "hs1.8xlarge"; } }
    }

    public struct AwsT1InstanceType
    {
        public string Micro { get { return "t1.micro"; } }
    }

    public struct AwsHI1InstanceType
    {
        public string X4Large { get { return "hi1.4xlarge"; } }
    }

    public struct AwsCR1InstanceType
    {
        public string X8Large { get { return "cr1.8xlarge"; } }
    }

    public struct AwsM2InstanceType
    {
        public string XLarge { get { return "m2.xlarge"; } }
        public string X2Large { get { return "m2.2xlarge"; } }
        public string X4Large { get { return "m2.4xlarge"; } }
    }

    public struct AwsCG1InstanceType
    {
        public const string cg1_4xlarge = "cg1.4xlarge";
    }

    public struct AwsCC2InstanceType
    {
        public const string cc2_8xlarge = "cc2.8xlarge";
    }

    public struct AwsC1InstanceType
    {
        public string Medium { get { return "c1.medium"; } }
        public string XLarge { get { return "c1.xlarge"; } }
    }

    public struct AwsM1InstanceType
    {
        public string Small { get { return "m1.small"; } }
        public string Medium { get { return "m1.medium"; } }
        public string Large { get { return "m1.large"; } }
        public string XLarge { get { return "m1.xlarge"; } }
    }

}