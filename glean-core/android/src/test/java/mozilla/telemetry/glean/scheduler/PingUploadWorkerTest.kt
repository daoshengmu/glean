package mozilla.telemetry.glean.scheduler

import android.content.Context
import androidx.test.core.app.ApplicationProvider
import androidx.test.ext.junit.runners.AndroidJUnit4
import androidx.work.BackoffPolicy
import androidx.work.NetworkType
import androidx.work.WorkerParameters
import mozilla.telemetry.glean.config.Configuration
import mozilla.telemetry.glean.resetGlean
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.Mock
import org.mockito.MockitoAnnotations

@RunWith(AndroidJUnit4::class)
class PingUploadWorkerTest {

    @Mock
    var workerParams: WorkerParameters? = null

    private var pingUploadWorker: PingUploadWorker? = null

    @Before
    @Throws(Exception::class)
    fun setUp() {
        val context: Context = ApplicationProvider.getApplicationContext()
        MockitoAnnotations.initMocks(this)
        resetGlean(context, config = Configuration().copy(logPings = true))
        pingUploadWorker = PingUploadWorker(context, workerParams!!)
    }

    @Test
    fun testPingConfiguration() {
        // Set the constraints around which the worker can be run, in this case it
        // only requires that any network connection be available.
        val workRequest = PingUploadWorker.buildWorkRequest()
        val workSpec = workRequest.workSpec

        // verify constraints
        Assert.assertEquals(NetworkType.CONNECTED, workSpec.constraints.requiredNetworkType)
        Assert.assertEquals(BackoffPolicy.EXPONENTIAL, workSpec.backoffPolicy)
        Assert.assertTrue(workRequest.tags.contains(PingUploadWorker.PING_WORKER_TAG))
    }

    @Test
    fun testDoWorkSuccess() {
        val result = pingUploadWorker!!.doWork()
        Assert.assertTrue(result.toString().contains("Success"))
    }
}
